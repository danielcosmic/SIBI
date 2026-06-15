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
public class ActivoController : ControllerBase
{
    private readonly SibiDbContext _db;
    private readonly HistorialService _historial;

    public ActivoController(SibiDbContext db, HistorialService historial)
    {
        _db = db;
        _historial = historial;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? categoriaId,
        [FromQuery] string? estado,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20)
    {
        var query = _db.Activos
            .Include(a => a.Categoria)
            .Include(a => a.PlacaNavigation)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoAnterior)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
            query = query.Where(a =>
                a.Placa.Contains(busqueda) ||
                a.Marca.Contains(busqueda) ||
                a.Modelo.Contains(busqueda) ||
                a.NumSerial.Contains(busqueda) ||
                a.Articulo.Contains(busqueda));

        if (categoriaId.HasValue)
            query = query.Where(a => a.CategoriaId == categoriaId);

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(a => a.Estado == estado);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(a => a.Placa)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return Ok(new { total, pagina, tamano, items = items.Select(MapToDto) });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        var totalActivos = await _db.Activos.CountAsync(a => a.Estado != "Desecho");
        var enDesecho = await _db.Activos.CountAsync(a => a.Estado == "Desecho");

        var categorias = await _db.Categorias.ToListAsync();
        var conteosPorCategoria = await _db.Activos
            .Where(a => a.Estado != "Desecho")
            .GroupBy(a => a.CategoriaId)
            .Select(g => new { CategoriaId = g.Key, Cantidad = g.Count() })
            .ToListAsync();

        var porCategoria = categorias.Select(c => new CategoriaStatDto(
            c.Id,
            c.Nombre,
            c.Icono,
            conteosPorCategoria.FirstOrDefault(x => x.CategoriaId == c.Id)?.Cantidad ?? 0
        )).ToList();

        return Ok(new ActivoStatsDto(totalActivos, enDesecho, 0, porCategoria));
    }

    [HttpGet("{placa}")]
    public async Task<IActionResult> ObtenerPorPlaca(string placa)
    {
        var activo = await _db.Activos
            .Include(a => a.Categoria)
            .Include(a => a.PlacaNavigation)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoAnterior)
            .FirstOrDefaultAsync(a => a.Placa == placa);

        if (activo is null) return NotFound();
        return Ok(MapToDto(activo));
    }

    [HttpPost]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Crear([FromBody] CrearActivoRequest request)
    {
        if (await _db.Activos.AnyAsync(a => a.Placa == request.Placa))
            return Conflict(new { mensaje = "Ya existe un activo con esa placa." });

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var encargado = await _db.Encargados.FindAsync(request.EncargadoId);
            if (encargado is null)
                return BadRequest(new { mensaje = "El encargado seleccionado no existe." });

            if (!await _db.Placas.AnyAsync(p => p.Numero == request.Placa))
                _db.Placas.Add(new Placa { Numero = request.Placa, Tipo = request.TipoPlaca });

            var ubicacion = new Ubicacion
            {
                Actual = request.UbicacionActual,
                Anterior = request.UbicacionActual,
                EncargadoActualId = encargado.Id,
                EncargadoAnteriorId = encargado.Id
            };
            _db.Ubicaciones.Add(ubicacion);
            await _db.SaveChangesAsync();

            var activo = new Activo
            {
                Placa = request.Placa,
                Marca = request.Marca,
                Modelo = request.Modelo,
                NumSerial = request.NumSerial,
                Articulo = request.Articulo,
                CategoriaId = request.CategoriaId,
                Observaciones = request.Observaciones,
                UbicacionId = ubicacion.Id,
                Estado = "Activo"
            };
            _db.Activos.Add(activo);
            await _db.SaveChangesAsync();

            var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _historial.RegistrarAsync(activo.Placa, correo, "Creacion",
                $"{activo.Marca} {activo.Modelo} agregado al inventario.");

            await tx.CommitAsync();
            return CreatedAtAction(nameof(ObtenerPorPlaca), new { placa = activo.Placa }, new { placa = activo.Placa });
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    [HttpPut("{placa}")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Editar(string placa, [FromBody] EditarActivoRequest request)
    {
        var activo = await _db.Activos
            .Include(a => a.UbicacionNavigation)
            .FirstOrDefaultAsync(a => a.Placa == placa);

        if (activo is null) return NotFound();

        var encargado = await _db.Encargados.FindAsync(request.EncargadoId);
        if (encargado is null)
            return BadRequest(new { mensaje = "El encargado seleccionado no existe." });

        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ubicacionCambiada = activo.UbicacionNavigation.Actual != request.UbicacionActual;
        var encargadoCambiado = activo.UbicacionNavigation.EncargadoActualId != request.EncargadoId;

        if (ubicacionCambiada || encargadoCambiado)
        {
            var nuevaUbicacion = new Ubicacion
            {
                Actual = request.UbicacionActual,
                Anterior = activo.UbicacionNavigation.Actual,
                EncargadoActualId = encargado.Id,
                EncargadoAnteriorId = activo.UbicacionNavigation.EncargadoActualId
            };
            _db.Ubicaciones.Add(nuevaUbicacion);
            await _db.SaveChangesAsync();
            activo.UbicacionId = nuevaUbicacion.Id;

            if (ubicacionCambiada)
                await _historial.RegistrarAsync(placa, correo, "CambioUbicacion",
                    $"Ubicación cambiada a {request.UbicacionActual}.");
            if (encargadoCambiado)
                await _historial.RegistrarAsync(placa, correo, "CambioEncargado",
                    $"Encargado cambiado a {encargado.Nombre}.");
        }

        if (activo.Estado != request.Estado)
        {
            activo.FechaDesecho = request.Estado == "Desecho"
                ? DateOnly.FromDateTime(DateTime.Today)
                : null;
            activo.Estado = request.Estado;
            await _historial.RegistrarAsync(placa, correo, "CambioEstado",
                $"Estado cambiado a {request.Estado}.");
        }

        activo.Marca = request.Marca;
        activo.Modelo = request.Modelo;
        activo.NumSerial = request.NumSerial;
        activo.Articulo = request.Articulo;
        activo.CategoriaId = request.CategoriaId;
        activo.Observaciones = request.Observaciones;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{placa}")]
    [Authorize(Roles = "Administradora")]
    public async Task<IActionResult> Eliminar(string placa)
    {
        var activo = await _db.Activos.FindAsync(placa);
        if (activo is null) return NotFound();

        if (activo.Estado != "Desecho")
            return BadRequest(new { mensaje = "El activo debe estar en estado Desecho antes de eliminar." });

        var hoy = DateOnly.FromDateTime(DateTime.Today);
        if (activo.FechaDesecho is null || hoy.DayNumber - activo.FechaDesecho.Value.DayNumber < 365)
            return BadRequest(new { mensaje = "El activo debe llevar al menos 1 año en estado Desecho." });

        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _historial.RegistrarAsync(placa, correo, "Eliminacion", $"Activo {placa} eliminado definitivamente.");

        _db.Activos.Remove(activo);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("recientes")]
    public async Task<IActionResult> Recientes([FromQuery] int categoriaId, [FromQuery] int tamano = 5)
    {
        var placasRecientes = await _db.Historial
            .Where(h => h.TipoAccion == "Creacion")
            .Join(_db.Activos.Where(a => a.CategoriaId == categoriaId),
                  h => h.ActivoPlaca,
                  a => a.Placa,
                  (h, a) => new { h.ActivoPlaca, h.FechaHora })
            .OrderByDescending(x => x.FechaHora)
            .Take(tamano)
            .Select(x => x.ActivoPlaca)
            .ToListAsync();

        var activos = await _db.Activos
            .Include(a => a.Categoria)
            .Include(a => a.PlacaNavigation)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoAnterior)
            .Where(a => placasRecientes.Contains(a.Placa))
            .ToListAsync();

        var ordenados = placasRecientes
            .Select(p => activos.First(a => a.Placa == p))
            .Select(MapToDto)
            .ToList();

        return Ok(ordenados);
    }

    internal static ActivoDto MapToDto(Activo a) => new(
        a.Placa,
        a.PlacaNavigation.Tipo,
        a.Marca,
        a.Modelo,
        a.NumSerial,
        a.Articulo,
        a.CategoriaId,
        a.Categoria.Nombre,
        a.Observaciones,
        a.UbicacionNavigation.Actual,
        a.UbicacionNavigation.Anterior,
        a.UbicacionNavigation.EncargadoActualId,
        a.UbicacionNavigation.EncargadoActual.Nombre,
        a.UbicacionNavigation.EncargadoAnterior.Nombre,
        a.Estado,
        a.FechaDesecho
    );
}
