using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services;

public class AuthService
{
    private readonly SibiDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(SibiDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<LoginResponse?> LoginAsync(string correo, string contrasena)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo && u.Activo);

        if (usuario is null) return null;

        if (usuario.IntentosFallidos >= 3) return null;

        if (!BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
        {
            usuario.IntentosFallidos++;
            await _db.SaveChangesAsync();
            return null;
        }

        usuario.IntentosFallidos = 0;
        await _db.SaveChangesAsync();

        return new LoginResponse(
            GenerarToken(usuario),
            usuario.Correo,
            usuario.Nombre,
            usuario.Permisos,
            usuario.EsContrasenaTemporal
        );
    }

    public async Task<string?> RecuperarContrasenaAsync(string correo)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo && u.Activo);

        if (usuario is null) return null;

        var tempPassword = GenerarContrasenaAleatoria();
        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(tempPassword);
        usuario.EsContrasenaTemporal = true;
        usuario.IntentosFallidos = 0;
        await _db.SaveChangesAsync();

        return tempPassword;
    }

    public async Task<bool> CambiarContrasenaAsync(string correo, string nueva)
    {
        if (!ValidarFuerzaContrasena(nueva)) return false;

        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo && u.Activo);

        if (usuario is null) return false;

        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(nueva);
        usuario.EsContrasenaTemporal = false;
        await _db.SaveChangesAsync();
        return true;
    }

    // Mínimo 6 chars, al menos una mayúscula, minúscula y número (regla del mockup)
    public static bool ValidarFuerzaContrasena(string contrasena) =>
        contrasena.Length >= 6 &&
        Regex.IsMatch(contrasena, @"[A-Z]") &&
        Regex.IsMatch(contrasena, @"[a-z]") &&
        Regex.IsMatch(contrasena, @"[0-9]");

    private string GenerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Correo),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Permisos)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpireHours"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerarContrasenaAleatoria()
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
