namespace backend.Models;

public class Ubicacion
{
    public long Id { get; set; }
    public string Actual { get; set; } = string.Empty;
    public string Anterior { get; set; } = string.Empty;
    public Guid EncargadoActualId { get; set; }
    public Guid EncargadoAnteriorId { get; set; }

    public Encargado EncargadoActual { get; set; } = null!;
    public Encargado EncargadoAnterior { get; set; } = null!;
}
