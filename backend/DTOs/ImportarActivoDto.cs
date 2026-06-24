namespace backend.DTOs;

public record ImportarActivoFilaRequest(
    string Placa,
    string TipoPlaca,
    string Articulo,
    string Marca,
    string Modelo,
    string NumSerial,
    string CategoriaNombre,
    string UbicacionActual,
    string EncargadoNombre,
    string? Observaciones
);

public record ImportarActivoResultadoDto(
    int Fila,
    string Placa,
    bool Exitoso,
    string? Error
);
