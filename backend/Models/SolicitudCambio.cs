namespace backend.Models;

public class SolicitudCambio
{
    public int Id { get; set; }
    public string ActivoPlaca { get; set; } = string.Empty;
    public string SolicitanteCorreo { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; } = DateTime.Now;
    public string DatosNuevos { get; set; } = string.Empty; // JSON
    public string Estado { get; set; } = "Pendiente"; // Pendiente | Aprobada | Rechazada
    public string? RevisorCorreo { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public string? Comentario { get; set; }

    public Activo Activo { get; set; } = null!;
    public Usuario Solicitante { get; set; } = null!;
    public Usuario? Revisor { get; set; }
}
