using System.Security.Claims;
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
            .Select(u => new UsuarioDto(u.Correo, u.Nombre, u.Permisos, u.Activo, u.IntentosFallidos))
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

        var tempPassword = GenerarContrasenaTemp();

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

    [HttpPost("{correo}/desbloquear")]
    public async Task<IActionResult> Desbloquear(string correo)
    {
        var usuario = await _db.Usuarios.FindAsync(correo);
        if (usuario is null) return NotFound();

        var tempPassword = GenerarContrasenaTemp();
        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(tempPassword);
        usuario.EsContrasenaTemporal = true;
        usuario.IntentosFallidos = 0;
        await _db.SaveChangesAsync();

        return Ok(new { correo = usuario.Correo, contrasenaTemp = tempPassword });
    }

    [HttpPost("{correo}/reset-contrasena")]
    public async Task<IActionResult> ResetContrasena(string correo)
    {
        var correoActual = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (correo == correoActual)
            return BadRequest(new { mensaje = "No puedes restablecer tu propia contraseña desde aquí." });

        var usuario = await _db.Usuarios.FindAsync(correo);
        if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });

        var tempPassword = GenerarContrasenaTemp();
        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(tempPassword);
        usuario.EsContrasenaTemporal = true;
        usuario.IntentosFallidos = 0;
        await _db.SaveChangesAsync();

        return Ok(new { correo = usuario.Correo, contrasenaTemp = tempPassword });
    }

    [HttpDelete("{correo}")]
    public async Task<IActionResult> Eliminar(string correo)
    {
        var correoActual = User.FindFirstValue(ClaimTypes.NameIdentifier);

        const string cuentaSoporte = "soporte.eic@ucr.ac.cr";

        if (correo == cuentaSoporte)
            return BadRequest(new { mensaje = "La cuenta de soporte del sistema no puede ser eliminada." });

        if (correo == correoActual)
            return BadRequest(new { mensaje = "No puedes eliminar tu propia cuenta." });

        var usuario = await _db.Usuarios.FindAsync(correo);
        if (usuario is null) return NotFound();

        _db.Usuarios.Remove(usuario);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{correo}")]
    public async Task<IActionResult> Editar(string correo, [FromBody] EditarUsuarioRequest request)
    {
        const string cuentaSoporte = "soporte.eic@ucr.ac.cr";

        var usuario = await _db.Usuarios.FindAsync(correo);
        if (usuario is null) return NotFound();

        if (correo == cuentaSoporte && request.Permisos != "Administradora")
            return BadRequest(new { mensaje = "El rol de la cuenta de soporte no puede modificarse." });

        if (correo == cuentaSoporte && !request.Activo)
            return BadRequest(new { mensaje = "La cuenta principal no puede desactivarse." });

        usuario.Nombre = request.Nombre;
        usuario.Permisos = correo == cuentaSoporte ? "Administradora" : request.Permisos;
        usuario.Activo = correo == cuentaSoporte ? true : request.Activo;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static string GenerarContrasenaTemp()
    {
        const string upper = "ABCDEFGHJKMNPQRSTUVWXYZ";
        const string lower = "abcdefghjkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string all = upper + lower + digits;
        var rng = new Random();

        var chars = new char[8];
        chars[0] = upper[rng.Next(upper.Length)];
        chars[1] = lower[rng.Next(lower.Length)];
        chars[2] = digits[rng.Next(digits.Length)];
        for (int i = 3; i < 8; i++)
            chars[i] = all[rng.Next(all.Length)];

        return new string(chars.OrderBy(_ => rng.Next()).ToArray());
    }
}
