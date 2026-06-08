using backend.Data;
using backend.Models;

namespace backend.Services;

public class HistorialService
{
    private readonly SibiDbContext _db;

    public HistorialService(SibiDbContext db) => _db = db;

    public async Task RegistrarAsync(
        string activoPlaca,
        string usuarioCorreo,
        string tipoAccion,
        string? descripcion = null)
    {
        _db.Historial.Add(new Historial
        {
            ActivoPlaca = activoPlaca,
            UsuarioCorreo = usuarioCorreo,
            TipoAccion = tipoAccion,
            Descripcion = descripcion,
            FechaHora = DateTime.Now
        });
        await _db.SaveChangesAsync();
    }
}
