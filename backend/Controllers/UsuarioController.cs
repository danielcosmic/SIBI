using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administradora")]
public class UsuarioController : ControllerBase
{
    private readonly SibiDbContext _db;

    public UsuarioController(SibiDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuarios = await _db.Usuarios
            .Select(u => new UsuarioDto(u.Correo, u.Nombre, u.Permisos, u.Activo))
            .ToListAsync();
        return Ok(usuarios);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioRequest request)
    {
        if (!request.Correo.EndsWith("@ucr.ac.cr"))
            return BadRequest(new { mensaje = "Solo se permiten correos con dominio @ucr.ac.cr." });

        if (await _db.Usuarios.AnyAsync(u => u.Correo == request.Correo))
            return Conflict(new { mensaje = "Ya existe un usuario con ese correo." });

        var rng = new Random();
        var upper = "ABCDEFGHJKMNPQRSTUVWXYZ";
        var lower = "abcdefghjkmnpqrstuvwxyz";
        var digits = "23456789";
        var tempPassword = $"{upper[rng.Next(upper.Length)]}{lower[rng.Next(lower.Length)]}{digits[rng.Next(digits.Length)]}{rng.Next(100, 999)}";

        _db.Usuarios.Add(new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            Contrasena = BCrypt.Net.BCrypt.HashPassword(tempPassword),
            Permisos = request.Permisos,
            EsContrasenaTemporal = true
        });
        await _db.SaveChangesAsync();

        return Ok(new { correo = request.Correo, contrasenaTemp = tempPassword });
    }

    [HttpPut("{correo}")]
    public async Task<IActionResult> Editar(string correo, [FromBody] EditarUsuarioRequest request)
    {
        var usuario = await _db.Usuarios.FindAsync(correo);
        if (usuario is null) return NotFound();

        usuario.Nombre = request.Nombre;
        usuario.Permisos = request.Permisos;
        usuario.Activo = request.Activo;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
