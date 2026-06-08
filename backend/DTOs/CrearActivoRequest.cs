namespace backend.DTOs;

public record CrearActivoRequest(
    string Placa,
    string TipoPlaca,
    string Marca,
    string Modelo,
    string NumSerial,
    string Articulo,
    int CategoriaId,
    string? Observaciones,
    string UbicacionActual,
    string NombreEncargado,
    string RolEncargado
);
