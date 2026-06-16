using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistorialController : ControllerBase
{
    private readonly SibiDbContext _db;

    public HistorialController(SibiDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? usuarioNombre,
        [FromQuery] string? activoPlaca,
        [FromQuery] string? tipoAccion,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 50)
    {
        var query = _db.Historial
            .Include(h => h.Usuario)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(usuarioNombre))
            query = query.Where(h => h.Usuario.Nombre.Contains(usuarioNombre));

        if (!string.IsNullOrWhiteSpace(activoPlaca))
            query = query.Where(h => h.ActivoPlaca == activoPlaca);

        if (!string.IsNullOrWhiteSpace(tipoAccion))
            query = query.Where(h => h.TipoAccion == tipoAccion);

        if (desde.HasValue)
            query = query.Where(h => h.FechaHora >= desde);

        if (hasta.HasValue)
            query = query.Where(h => h.FechaHora <= hasta);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(h => h.FechaHora)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .Select(h => new HistorialDto(
                h.Id,
                h.ActivoPlaca,
                h.UsuarioCorreo,
                h.Usuario.Nombre,
                h.TipoAccion,
                h.Descripcion,
                h.FechaHora))
            .ToListAsync();

        return Ok(new { total, pagina, tamano, items });
    }
}
