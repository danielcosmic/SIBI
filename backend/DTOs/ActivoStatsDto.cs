namespace backend.DTOs;

public record ActivoStatsDto(
    int TotalActivos,
    int EnDesecho,
    int SolicitudesPendientes,
    IEnumerable<CategoriaStatDto> PorCategoria
);

public record CategoriaStatDto(int Id, string Nombre, string? Icono, int Cantidad);
