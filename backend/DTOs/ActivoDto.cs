namespace backend.DTOs;

public record ActivoDto(
    string Placa,
    string TipoPlaca,
    string Marca,
    string Modelo,
    string NumSerial,
    string Articulo,
    int CategoriaId,
    string CategoriaNombre,
    string? Observaciones,
    string UbicacionActual,
    string UbicacionAnterior,
    Guid EncargadoActualId,
    string EncargadoActual,
    string EncargadoAnterior,
    string Estado,
    DateOnly? FechaDesecho
);
