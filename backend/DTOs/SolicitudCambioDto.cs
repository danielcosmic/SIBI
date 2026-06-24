namespace backend.DTOs;

public record SolicitudCambioDto(
    int Id,
    string ActivoPlaca,
    string ArticuloActual,
    string MarcaActual,
    string ModeloActual,
    string UbicacionActual,
    string EncargadoActual,
    string SolicitanteNombre,
    string SolicitanteCorreo,
    DateTime FechaSolicitud,
    string Estado,
    SolicitudDatosDto DatosNuevos,
    string? RevisorNombre,
    DateTime? FechaResolucion,
    string? Comentario
);

public record SolicitudDatosDto(
    string Marca,
    string Modelo,
    string NumSerial,
    string Articulo,
    int CategoriaId,
    string CategoriaNombre,
    string? Observaciones,
    string UbicacionActual,
    Guid EncargadoId,
    string EncargadoNombre,
    string Estado,
    // Snapshot del activo al momento de crear la solicitud
    string? MarcaOriginal,
    string? ModeloOriginal,
    string? ArticuloOriginal,
    string? UbicacionOriginal,
    string? EncargadoNombreOriginal
);

public record CrearSolicitudRequest(
    string ActivoPlaca,
    string Marca,
    string Modelo,
    string NumSerial,
    string Articulo,
    int CategoriaId,
    string? Observaciones,
    string UbicacionActual,
    Guid EncargadoId,
    string Estado
);

public record RechazarSolicitudRequest(string? Comentario);
