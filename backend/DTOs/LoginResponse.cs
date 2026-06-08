namespace backend.DTOs;

public record LoginResponse(
    string Token,
    string Correo,
    string Nombre,
    string Permisos,
    bool EsContrasenaTemporal
);
