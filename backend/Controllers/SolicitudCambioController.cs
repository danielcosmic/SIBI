using System.Security.Claims;
using System.Text.Json;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitudCambioController : ControllerBase
{
    private readonly SibiDbContext _db;
    private readonly HistorialService _historial;
    private readonly NotificacionService _notif;

    public SolicitudCambioController(SibiDbContext db, HistorialService historial, NotificacionService notif)
    {
        _db = db;
        _historial = historial;
        _notif = notif;
    }

    [HttpGet]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Listar([FromQuery] string? estado)
    {
        var query = _db.SolicitudesCambio
            .Include(s => s.Activo).ThenInclude(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .Include(s => s.Solicitante)
            .Include(s => s.Revisor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(s => s.Estado == estado);

        var lista = await query.OrderByDescending(s => s.FechaSolicitud).ToListAsync();
        return Ok(lista.Select(MapToDto));
    }

    [HttpGet("mis")]
    [Authorize(Roles = "JefaAdministrativa")]
    public async Task<IActionResult> ListarMias([FromQuery] string? estado)
    {
        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var query = _db.SolicitudesCambio
            .Include(s => s.Activo).ThenInclude(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .Include(s => s.Solicitante)
            .Include(s => s.Revisor)
            .Where(s => s.SolicitanteCorreo == correo);

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(s => s.Estado == estado);

        var lista = await query.OrderByDescending(s => s.FechaSolicitud).ToListAsync();
        return Ok(lista.Select(MapToDto));
    }

    [HttpGet("activo/{placa}/pendiente")]
    [Authorize(Roles = "JefaAdministrativa")]
    public async Task<IActionResult> ObtenerPendienteDeActivo(string placa)
    {
        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var solicitud = await _db.SolicitudesCambio
            .Include(s => s.Activo).ThenInclude(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .Include(s => s.Solicitante)
            .Include(s => s.Revisor)
            .FirstOrDefaultAsync(s => s.ActivoPlaca == placa
                                   && s.SolicitanteCorreo == correo
                                   && s.Estado == "Pendiente");

        if (solicitud is null) return NoContent();
        return Ok(MapToDto(solicitud));
    }

    [HttpGet("pendientes/count")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> ContarPendientes()
    {
        var count = await _db.SolicitudesCambio.CountAsync(s => s.Estado == "Pendiente");
        return Ok(new { count });
    }

    [HttpPost]
    [Authorize(Roles = "JefaAdministrativa")]
    public async Task<IActionResult> Crear([FromBody] CrearSolicitudRequest request)
    {
        var activo = await _db.Activos
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .FirstOrDefaultAsync(a => a.Placa == request.ActivoPlaca);
        if (activo is null) return NotFound(new { mensaje = "Activo no encontrado." });

        var categoria = await _db.Categorias.FindAsync(request.CategoriaId);
        var encargado = await _db.Encargados.FindAsync(request.EncargadoId);
        if (encargado is null) return BadRequest(new { mensaje = "Encargado no encontrado." });

        var datos = new SolicitudDatosDto(
            request.Marca, request.Modelo, request.NumSerial, request.Articulo,
            request.CategoriaId, categoria?.Nombre ?? "",
            request.Observaciones, request.UbicacionActual,
            request.EncargadoId, encargado.Nombre,
            request.Estado,
            activo.Marca,
            activo.Modelo,
            activo.Articulo,
            activo.UbicacionNavigation?.Actual,
            activo.UbicacionNavigation?.EncargadoActual?.Nombre
        );

        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var solicitud = new SolicitudCambio
        {
            ActivoPlaca = request.ActivoPlaca,
            SolicitanteCorreo = correo,
            DatosNuevos = JsonSerializer.Serialize(datos),
            Estado = "Pendiente"
        };

        _db.SolicitudesCambio.Add(solicitud);
        await _db.SaveChangesAsync();

        var nombreSolicitante = User.FindFirstValue(ClaimTypes.Name);
        await _historial.RegistrarAsync(request.ActivoPlaca, correo, "SolicitudCambio",
            $"Solicitud de cambio enviada por {nombreSolicitante}.");
        await _notif.NotificarGTIAdminAsync("solicitud_cambio", "Nueva solicitud de cambio",
            $"{nombreSolicitante} solicitó cambios en el activo {request.ActivoPlaca}.");

        return Ok(new { id = solicitud.Id });
    }

    [HttpPost("{id}/aprobar")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Aprobar(int id)
    {
        var solicitud = await _db.SolicitudesCambio
            .Include(s => s.Activo).ThenInclude(a => a.UbicacionNavigation)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (solicitud is null) return NotFound();
        if (solicitud.Estado != "Pendiente")
            return BadRequest(new { mensaje = "La solicitud ya fue procesada." });

        var datos = JsonSerializer.Deserialize<SolicitudDatosDto>(solicitud.DatosNuevos)!;
        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Aplicar cambios al activo
        var activo = solicitud.Activo;
        activo.Marca = datos.Marca;
        activo.Modelo = datos.Modelo;
        activo.NumSerial = datos.NumSerial;
        activo.Articulo = datos.Articulo;
        activo.CategoriaId = datos.CategoriaId;
        activo.Observaciones = datos.Observaciones;
        activo.Estado = datos.Estado;

        if (activo.UbicacionNavigation.Actual != datos.UbicacionActual ||
            activo.UbicacionNavigation.EncargadoActualId != datos.EncargadoId)
        {
            var nuevaUbicacion = new Ubicacion
            {
                Actual = datos.UbicacionActual,
                Anterior = activo.UbicacionNavigation.Actual,
                EncargadoActualId = datos.EncargadoId,
                EncargadoAnteriorId = activo.UbicacionNavigation.EncargadoActualId
            };
            _db.Ubicaciones.Add(nuevaUbicacion);
            await _db.SaveChangesAsync();
            activo.UbicacionId = nuevaUbicacion.Id;
        }

        solicitud.Estado = "Aprobada";
        solicitud.RevisorCorreo = correo;
        solicitud.FechaResolucion = DateTime.Now;
        await _db.SaveChangesAsync();

        await _historial.RegistrarAsync(activo.Placa, correo, "SolicitudAprobada",
            $"Cambios aprobados por {User.FindFirstValue(ClaimTypes.Name)}.");

        return NoContent();
    }

    [HttpPost("{id}/rechazar")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Rechazar(int id, [FromBody] RechazarSolicitudRequest request)
    {
        var solicitud = await _db.SolicitudesCambio.FindAsync(id);
        if (solicitud is null) return NotFound();
        if (solicitud.Estado != "Pendiente")
            return BadRequest(new { mensaje = "La solicitud ya fue procesada." });

        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        solicitud.Estado = "Rechazada";
        solicitud.RevisorCorreo = correo;
        solicitud.FechaResolucion = DateTime.Now;
        solicitud.Comentario = request.Comentario;
        await _db.SaveChangesAsync();

        await _historial.RegistrarAsync(solicitud.ActivoPlaca, correo, "SolicitudRechazada",
            $"Solicitud rechazada. {request.Comentario}");

        return NoContent();
    }

    private static SolicitudCambioDto MapToDto(SolicitudCambio s)
    {
        var datos = JsonSerializer.Deserialize<SolicitudDatosDto>(s.DatosNuevos)!;
        var ub = s.Activo.UbicacionNavigation;
        return new SolicitudCambioDto(
            s.Id,
            s.ActivoPlaca,
            datos.ArticuloOriginal ?? s.Activo.Articulo,
            datos.MarcaOriginal ?? s.Activo.Marca,
            datos.ModeloOriginal ?? s.Activo.Modelo,
            datos.UbicacionOriginal ?? ub?.Actual ?? "",
            datos.EncargadoNombreOriginal ?? ub?.EncargadoActual?.Nombre ?? "",
            s.Solicitante.Nombre,
            s.SolicitanteCorreo,
            s.FechaSolicitud,
            s.Estado,
            datos,
            s.Revisor?.Nombre,
            s.FechaResolucion,
            s.Comentario
        );
    }
}
