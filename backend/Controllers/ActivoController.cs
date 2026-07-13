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
        [FromQuery] int[]? categoriaIds,
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

        if (categoriaIds is { Length: > 0 })
            query = query.Where(a => categoriaIds.Contains(a.CategoriaId));

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
        var totalActivos = await _db.Activos.CountAsync(a => a.Estado == "Activo");
        var enDesecho = await _db.Activos.CountAsync(a => a.Estado == "Desecho");
        var enMantenimiento = await _db.Activos.CountAsync(a => a.Estado == "Mantenimiento");

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

        return Ok(new ActivoStatsDto(totalActivos, enDesecho, enMantenimiento, 0, porCategoria));
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

    [HttpPost("importar")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Importar([FromBody] List<ImportarActivoFilaRequest> filas)
    {
        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var categoriasPorNombre = await _db.Categorias
            .ToDictionaryAsync(c => c.Nombre.ToLower(), c => c.Id);

        var todosEncargados = await _db.Encargados.ToListAsync();
        var encargadosPorNombre = todosEncargados
            .GroupBy(e => e.Nombre.ToLower())
            .ToDictionary(g => g.Key, g => g.ToList());

        var resultados = new List<ImportarActivoResultadoDto>();

        for (int i = 0; i < filas.Count; i++)
        {
            var fila = filas[i];
            int numFila = i + 2;

            if (string.IsNullOrWhiteSpace(fila.Placa))
            {
                resultados.Add(new ImportarActivoResultadoDto(numFila, "(vacía)", false, "La placa es obligatoria."));
                continue;
            }

            if (await _db.Activos.AnyAsync(a => a.Placa == fila.Placa))
            {
                resultados.Add(new ImportarActivoResultadoDto(numFila, fila.Placa, false, "Ya existe un activo con esa placa."));
                continue;
            }

            if (!categoriasPorNombre.TryGetValue(fila.CategoriaNombre.ToLower(), out var categoriaId))
            {
                resultados.Add(new ImportarActivoResultadoDto(numFila, fila.Placa, false, $"Categoría '{fila.CategoriaNombre}' no encontrada."));
                continue;
            }

            Guid encargadoId;
            if (fila.EncargadoId.HasValue)
            {
                if (!await _db.Encargados.AnyAsync(e => e.Id == fila.EncargadoId.Value))
                {
                    resultados.Add(new ImportarActivoResultadoDto(numFila, fila.Placa, false, "El encargado seleccionado ya no existe en el sistema."));
                    continue;
                }
                encargadoId = fila.EncargadoId.Value;
            }
            else
            {
                if (!encargadosPorNombre.TryGetValue(fila.EncargadoNombre.ToLower(), out var candidatos) || candidatos.Count == 0)
                {
                    resultados.Add(new ImportarActivoResultadoDto(numFila, fila.Placa, false, $"Encargado '{fila.EncargadoNombre}' no encontrado."));
                    continue;
                }
                if (candidatos.Count > 1)
                {
                    resultados.Add(new ImportarActivoResultadoDto(numFila, fila.Placa, false, $"Nombre '{fila.EncargadoNombre}' coincide con varios encargados en el sistema."));
                    continue;
                }
                encargadoId = candidatos[0].Id;
            }

            string? activoDescripcion = null;
            var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                if (!await _db.Placas.AnyAsync(p => p.Numero == fila.Placa))
                    _db.Placas.Add(new Placa { Numero = fila.Placa, Tipo = fila.TipoPlaca });

                var ubicacion = new Ubicacion
                {
                    Actual = fila.UbicacionActual,
                    Anterior = fila.UbicacionActual,
                    EncargadoActualId = encargadoId,
                    EncargadoAnteriorId = encargadoId
                };
                _db.Ubicaciones.Add(ubicacion);
                await _db.SaveChangesAsync();

                var activo = new Activo
                {
                    Placa = fila.Placa,
                    Marca = fila.Marca,
                    Modelo = fila.Modelo,
                    NumSerial = fila.NumSerial,
                    Articulo = fila.Articulo,
                    CategoriaId = categoriaId,
                    Observaciones = string.IsNullOrWhiteSpace(fila.Observaciones) ? null : fila.Observaciones,
                    UbicacionId = ubicacion.Id,
                    Estado = "Activo"
                };
                _db.Activos.Add(activo);
                await _db.SaveChangesAsync();

                activoDescripcion = $"{activo.Marca} {activo.Modelo}";
                await tx.CommitAsync();
                resultados.Add(new ImportarActivoResultadoDto(numFila, fila.Placa, true, null));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                // Detach only entities in Added/Modified state to avoid polluting subsequent iterations.
                foreach (var entry in _db.ChangeTracker.Entries()
                    .Where(e => e.State is EntityState.Added or EntityState.Modified)
                    .ToList())
                    entry.State = EntityState.Detached;
                var detalle = ex.InnerException?.Message ?? ex.Message;
                string mensaje;
                if (detalle.Contains("CHK_Placa_Tipo"))
                    mensaje = "El tipo de placa no es válido. Use 'Institucional' o 'Interno'.";
                else if (detalle.Contains("NumSerial") && (detalle.Contains("UNIQUE") || detalle.Contains("duplicate")))
                    mensaje = "El número de serie ya está registrado en otro activo.";
                else if (detalle.Contains("PRIMARY KEY") || (detalle.Contains("UNIQUE") && detalle.Contains("Placa")) || detalle.Contains("duplicate key"))
                    mensaje = "La placa ya existe en el inventario.";
                else if (detalle.Contains("UNIQUE") || detalle.Contains("duplicate"))
                    mensaje = "Un campo único (placa o número de serie) está duplicado.";
                else if (detalle.Contains("FK_Activos_Categorias") || detalle.Contains("CategoriaId"))
                    mensaje = "La categoría indicada no existe en el sistema.";
                else if (detalle.Contains("FK_") && (detalle.Contains("Encargado") || detalle.Contains("Ubicacion")))
                    mensaje = "El encargado o ubicación indicado no existe en el sistema.";
                else if (detalle.Contains("FOREIGN KEY") || detalle.Contains("FK_"))
                    mensaje = "Un valor referenciado (categoría, encargado o ubicación) no existe en el sistema.";
                else if (detalle.Contains("String or binary data would be truncated") || detalle.Contains("truncat"))
                {
                    var matchCol = System.Text.RegularExpressions.Regex.Match(detalle, @"column '(\w+)'");
                    if (matchCol.Success)
                    {
                        var colEsp = matchCol.Groups[1].Value switch
                        {
                            "Marca"         => "Marca",
                            "Modelo"        => "Modelo",
                            "NumSerial"     => "N° Serial",
                            "Articulo"      => "Artículo",
                            "Observaciones" => "Observaciones",
                            "Actual"        => "Ubicación actual",
                            "Anterior"      => "Ubicación anterior",
                            var c           => c
                        };
                        mensaje = $"El campo '{colEsp}' excede la longitud máxima permitida.";
                    }
                    else
                        mensaje = "Uno de los campos de texto excede la longitud máxima permitida.";
                }
                else if (detalle.Contains("NOT NULL") || detalle.Contains("cannot be null"))
                    mensaje = "Faltan campos obligatorios.";
                else
                    mensaje = "Error al guardar el activo. Revise que los datos sean válidos y no existan duplicados.";
                resultados.Add(new ImportarActivoResultadoDto(numFila, fila.Placa, false, mensaje));
            }
            finally
            {
                // Dispose the transaction before registering historial so the DbContext
                // has no active transaction when HistorialService calls SaveChangesAsync.
                await tx.DisposeAsync();
            }

            if (activoDescripcion is not null)
            {
                try
                {
                    await _historial.RegistrarAsync(fila.Placa, correo, "Creacion",
                        $"{activoDescripcion} agregado al inventario (importación masiva).");
                }
                catch { /* Historial is best-effort; import already committed. */ }
                finally
                {
                    // Detach any Historial entities so a failed save doesn't leak into the
                    // next iteration's SaveChangesAsync inside the activo transaction.
                    foreach (var e in _db.ChangeTracker.Entries<Historial>().ToList())
                        e.State = EntityState.Detached;
                }
            }
        }

        return Ok(resultados);
    }

    [HttpPut("{placa}")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> Editar(string placa, [FromBody] EditarActivoRequest request)
    {
        var activo = await _db.Activos
            .Include(a => a.UbicacionNavigation).ThenInclude(u => u.EncargadoActual)
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
            var ubicacionAnterior = activo.UbicacionNavigation.Actual;
            var encargadoAnteriorNombre = activo.UbicacionNavigation.EncargadoActual.Nombre;

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
                    $"Ubicación: {ubicacionAnterior} -> {request.UbicacionActual}");
            if (encargadoCambiado)
                await _historial.RegistrarAsync(placa, correo, "CambioEncargado",
                    $"Encargado: {encargadoAnteriorNombre} -> {encargado.Nombre}");
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

    [HttpPatch("{placa}/cambiar-placa")]
    [Authorize(Roles = "GTI,Administradora")]
    public async Task<IActionResult> CambiarPlaca(string placa, [FromBody] CambiarPlacaRequest request)
    {
        var nuevaPlaca = request.NuevaPlaca?.Trim();
        if (string.IsNullOrWhiteSpace(nuevaPlaca))
            return BadRequest(new { mensaje = "La nueva placa es obligatoria." });

        if (nuevaPlaca == placa)
            return BadRequest(new { mensaje = "La nueva placa debe ser diferente a la actual." });

        if (await _db.Activos.AnyAsync(a => a.Placa == nuevaPlaca))
            return Conflict(new { mensaje = "Ya existe un activo con esa placa." });

        var activo = await _db.Activos.FindAsync(placa);
        if (activo is null) return NotFound();

        var correo = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var placaAntigua = await _db.Placas.FindAsync(placa);

            // New Placa entry + new Activo copy
            _db.Placas.Add(new Placa { Numero = nuevaPlaca, Tipo = placaAntigua?.Tipo ?? "Institucional" });
            _db.Activos.Add(new Activo
            {
                Placa        = nuevaPlaca,
                Marca        = activo.Marca,
                Modelo       = activo.Modelo,
                NumSerial    = activo.NumSerial,
                Articulo     = activo.Articulo,
                CategoriaId  = activo.CategoriaId,
                Observaciones = activo.Observaciones,
                UbicacionId  = activo.UbicacionId,
                Estado       = activo.Estado,
                FechaDesecho = activo.FechaDesecho
            });
            await _db.SaveChangesAsync();

            // Re-point dependents to the new placa
            var historialEntries = await _db.Historial.Where(h => h.ActivoPlaca == placa).ToListAsync();
            foreach (var h in historialEntries) h.ActivoPlaca = nuevaPlaca;

            var solicitudes = await _db.SolicitudesCambio.Where(s => s.ActivoPlaca == placa).ToListAsync();
            foreach (var s in solicitudes) s.ActivoPlaca = nuevaPlaca;

            await _db.SaveChangesAsync();

            // Remove old Activo and old Placa (no dependents left)
            _db.Activos.Remove(activo);
            if (placaAntigua is not null) _db.Placas.Remove(placaAntigua);
            await _db.SaveChangesAsync();

            await _historial.RegistrarAsync(nuevaPlaca, correo, "CambioPlaca",
                $"Placa cambiada de {placa} a {nuevaPlaca}.");

            await tx.CommitAsync();
            return Ok(new { placa = nuevaPlaca });
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
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

        // Cargar todas las dependencias antes de encolar los borrados
        var historialEntries = await _db.Historial
            .Where(h => h.ActivoPlaca == placa).ToListAsync();
        var solicitudes = await _db.SolicitudesCambio
            .Where(s => s.ActivoPlaca == placa).ToListAsync();
        var placaEntity = await _db.Placas.FindAsync(placa);
        var ubicacion   = await _db.Ubicaciones.FindAsync(activo.UbicacionId);

        // Encolar borrados: EF ordena los DELETE respetando las FK (dependientes primero)
        _db.Historial.RemoveRange(historialEntries);
        _db.SolicitudesCambio.RemoveRange(solicitudes);
        _db.Activos.Remove(activo);
        if (placaEntity is not null) _db.Placas.Remove(placaEntity);
        if (ubicacion   is not null) _db.Ubicaciones.Remove(ubicacion);

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
