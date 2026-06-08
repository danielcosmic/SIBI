namespace backend.DTOs;

public record HistorialDto(
    long Id,
    string ActivoPlaca,
    string UsuarioCorreo,
    string UsuarioNombre,
    string TipoAccion,
    string? Descripcion,
    DateTime FechaHora
);
