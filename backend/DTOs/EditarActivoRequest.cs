namespace backend.DTOs;

public record EditarActivoRequest(
    string Marca,
    string Modelo,
    string NumSerial,
    string Articulo,
    int CategoriaId,
    string? Observaciones,
    string UbicacionActual,
    string NombreEncargado,
    string RolEncargado,
    string Estado
);
