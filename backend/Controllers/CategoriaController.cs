using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly SibiDbContext _db;

    public CategoriaController(SibiDbContext db) => _db = db;

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Listar()
    {
        var categorias = await _db.Categorias
            .Select(c => new CategoriaDto(c.Id, c.Nombre, c.Icono))
            .ToListAsync();
        return Ok(categorias);
    }

    [HttpPost]
    [Authorize(Roles = "Administradora")]
    public async Task<IActionResult> Crear([FromBody] CrearCategoriaRequest request)
    {
        if (await _db.Categorias.AnyAsync(c => c.Nombre == request.Nombre))
            return Conflict(new { mensaje = "Ya existe una categoría con ese nombre." });

        var categoria = new Categoria { Nombre = request.Nombre, Icono = request.Icono };
        _db.Categorias.Add(categoria);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Listar), new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Icono));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administradora")]
    public async Task<IActionResult> Editar(int id, [FromBody] CrearCategoriaRequest request)
    {
        var categoria = await _db.Categorias.FindAsync(id);
        if (categoria is null) return NotFound(new { mensaje = "Categoría no encontrada." });

        if (await _db.Categorias.AnyAsync(c => c.Nombre == request.Nombre && c.Id != id))
            return Conflict(new { mensaje = "Ya existe una categoría con ese nombre." });

        categoria.Nombre = request.Nombre;
        categoria.Icono = request.Icono;
        await _db.SaveChangesAsync();
        return Ok(new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Icono));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administradora")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var categoria = await _db.Categorias.FindAsync(id);
        if (categoria is null) return NotFound();

        if (await _db.Activos.AnyAsync(a => a.CategoriaId == id))
            return BadRequest(new { mensaje = "No se puede eliminar una categoría con activos asociados." });

        _db.Categorias.Remove(categoria);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
