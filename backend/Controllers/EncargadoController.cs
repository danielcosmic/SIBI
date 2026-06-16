using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EncargadoController : ControllerBase
{
    private readonly SibiDbContext _db;
    private readonly HistorialService _historial;

    public EncargadoController(SibiDbContext db, HistorialService historial)
    {
        _db = db;
        _historial = historial;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var encargados = await _db.Encargados
            .OrderBy(e => e.Nombre)
            .Select(e => new EncargadoDto(
                e.Id,
                e.Nombre,
                e.Rol,
                _db.Activos.Count(a => a.UbicacionNavigation.EncargadoActualId == e.Id)))
            .ToListAsync();
        return Ok(encargados);
    }

    [HttpPost]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Crear([FromBody] CrearEncargadoRequest request)
    {
        if (await _db.Encargados.AnyAsync(e => e.Nombre == request.Nombre && e.Rol == request.Rol))
            return Conflict(new { mensaje = "Ya existe un encargado con ese nombre y rol." });

        var encargado = new Encargado { Nombre = request.Nombre, Rol = request.Rol };
        _db.Encargados.Add(encargado);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Listar), new EncargadoDto(encargado.Id, encargado.Nombre, encargado.Rol, 0));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarEncargadoRequest request)
    {
        var encargado = await _db.Encargados.FindAsync(id);
        if (encargado is null) return NotFound();

        if (await _db.Encargados.AnyAsync(e => e.Nombre == request.Nombre && e.Rol == request.Rol && e.Id != id))
            return Conflict(new { mensaje = "Ya existe un encargado con ese nombre y rol." });

        encargado.Nombre = request.Nombre;
        encargado.Rol = request.Rol;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administradora")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var encargado = await _db.Encargados.FindAsync(id);
        if (encargado is null) return NotFound();

        var referenciado = await _db.Ubicaciones.AnyAsync(
            u => u.EncargadoActualId == id || u.EncargadoAnteriorId == id);
        if (referenciado)
            return BadRequest(new { mensaje = "No se puede eliminar un encargado que tiene activos asignados." });

        _db.Encargados.Remove(encargado);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:guid}/activos")]
    public async Task<IActionResult> ActivosDelEncargado(Guid id)
    {
        var existe = await _db.Encargados.AnyAsync(e => e.Id == id);
        if (!existe) return NotFound();

        var activos = await _db.Activos
            .Include(a => a.Categoria)
            .Include(a => a.PlacaNavigation)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoAnterior)
            .Where(a => a.UbicacionNavigation.EncargadoActualId == id)
            .OrderBy(a => a.Placa)
            .ToListAsync();

        return Ok(activos.Select(ActivoController.MapToDto));
    }

    [HttpPost("{id:guid}/reasignar")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> ReasignarActivos(Guid id, [FromBody] ReasignarActivosRequest request)
    {
        var destino = await _db.Encargados.FindAsync(request.NuevoEncargadoId);
        if (destino is null)
            return BadRequest(new { mensaje = "El encargado de destino no existe." });

        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        foreach (var placa in request.Placas)
        {
            var activo = await _db.Activos
                .Include(a => a.UbicacionNavigation)
                .FirstOrDefaultAsync(a => a.Placa == placa);
            if (activo is null) continue;

            var nuevaUbicacion = new Ubicacion
            {
                Actual = activo.UbicacionNavigation.Actual,
                Anterior = activo.UbicacionNavigation.Actual,
                EncargadoActualId = request.NuevoEncargadoId,
                EncargadoAnteriorId = activo.UbicacionNavigation.EncargadoActualId
            };
            _db.Ubicaciones.Add(nuevaUbicacion);
            await _db.SaveChangesAsync();

            activo.UbicacionId = nuevaUbicacion.Id;
            await _historial.RegistrarAsync(placa, correo, "CambioEncargado",
                $"Encargado cambiado a {destino.Nombre}.");
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
