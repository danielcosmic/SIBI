namespace backend.Models;

public class Historial
{
    public long Id { get; set; }
    public string ActivoPlaca { get; set; } = string.Empty;
    public string UsuarioCorreo { get; set; } = string.Empty;
    public string TipoAccion { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaHora { get; set; }

    public Activo Activo { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
