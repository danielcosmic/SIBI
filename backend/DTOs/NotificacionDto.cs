namespace backend.DTOs;

public record NotificacionDto(
    string Tipo,
    string Titulo,
    string Mensaje,
    DateTime Fecha
);
