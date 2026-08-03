using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DesechoController : ControllerBase
{
    private readonly SibiDbContext _db;
    private readonly HistorialService _historial;

    public DesechoController(SibiDbContext db, HistorialService historial)
    {
        _db = db;
        _historial = historial;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var activos = await _db.Activos
            .Where(a => a.Estado == "Desecho")
            .Include(a => a.Categoria)
            .Include(a => a.PlacaNavigation)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoAnterior)
            .ToListAsync();

        var hoy = DateOnly.FromDateTime(DateTime.Today);

        return Ok(activos.Select(a => new
        {
            activo = ActivoController.MapToDto(a),
            diasEnDesecho = a.FechaDesecho is not null
                ? hoy.DayNumber - a.FechaDesecho.Value.DayNumber
                : 0,
            puedeEliminar = a.FechaDesecho is not null &&
                hoy.DayNumber - a.FechaDesecho.Value.DayNumber >= 365
        }));
    }

    [HttpPost("{placa}/aprobar")]
    [Authorize(Roles = "Administradora")]
    public async Task<IActionResult> AprobarEliminacion(string placa)
    {
        var activo = await _db.Activos.FindAsync(placa);
        if (activo is null) return NotFound();

        if (activo.Estado != "Desecho")
            return BadRequest(new { mensaje = "El activo no está en estado Desecho." });

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        if (activo.FechaDesecho is null || hoy.DayNumber - activo.FechaDesecho.Value.DayNumber < 365)
            return BadRequest(new { mensaje = "El activo debe llevar al menos 1 año en estado Desecho." });

        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _historial.RegistrarAsync(placa, correo, "Aprobacion", "Eliminación definitiva aprobada.");

        // Load all dependents (including the "Aprobacion" entry just saved) before deleting
        var historialEntries = await _db.Historial.Where(h => h.ActivoPlaca == placa).ToListAsync();
        var solicitudes = await _db.SolicitudesCambio.Where(s => s.ActivoPlaca == placa).ToListAsync();
        var placaEntity = await _db.Placas.FindAsync(placa);
        var ubicacion   = await _db.Ubicaciones.FindAsync(activo.UbicacionId);

        _db.Historial.RemoveRange(historialEntries);
        _db.SolicitudesCambio.RemoveRange(solicitudes);
        _db.Activos.Remove(activo);
        if (placaEntity is not null) _db.Placas.Remove(placaEntity);
        if (ubicacion   is not null) _db.Ubicaciones.Remove(ubicacion);

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{placa}/rechazar")]
    [Authorize(Roles = "Administradora")]
    public async Task<IActionResult> RechazarEliminacion(string placa)
    {
        var activo = await _db.Activos.FindAsync(placa);
        if (activo is null) return NotFound();

        if (activo.Estado != "Desecho")
            return BadRequest(new { mensaje = "El activo no está en estado Desecho." });

        activo.Estado = "Activo";
        activo.FechaDesecho = null;
        await _db.SaveChangesAsync();

        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _historial.RegistrarAsync(placa, correo, "Rechazo", "Solicitud de eliminación rechazada, activo restaurado.");
        return NoContent();
    }
}
