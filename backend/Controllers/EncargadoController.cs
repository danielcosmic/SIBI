using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EncargadoController : ControllerBase
{
    private readonly SibiDbContext _db;
    public EncargadoController(SibiDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var encargados = await _db.Encargados
            .OrderBy(e => e.Nombre)
            .Select(e => new EncargadoDto(e.Id, e.Nombre, e.Rol))
            .ToListAsync();
        return Ok(encargados);
    }

    [HttpPost]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Crear([FromBody] CrearEncargadoRequest request)
    {
        var encargado = new Encargado { Nombre = request.Nombre, Rol = request.Rol };
        _db.Encargados.Add(encargado);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Listar), new EncargadoDto(encargado.Id, encargado.Nombre, encargado.Rol));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarEncargadoRequest request)
    {
        var encargado = await _db.Encargados.FindAsync(id);
        if (encargado is null) return NotFound();

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
}
