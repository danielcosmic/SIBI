using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

public class Activo
{
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string NumSerial { get; set; } = string.Empty;
    public string Articulo { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string? Observaciones { get; set; }

    [Column("Ubicacion")]
    public long UbicacionId { get; set; }

    public string Estado { get; set; } = "Activo"; // Activo | Mantenimiento | Desecho
    public DateOnly? FechaDesecho { get; set; }

    public Placa PlacaNavigation { get; set; } = null!;
    public Categoria Categoria { get; set; } = null!;
    public Ubicacion UbicacionNavigation { get; set; } = null!;
}
