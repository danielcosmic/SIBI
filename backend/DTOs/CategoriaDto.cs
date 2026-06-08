namespace backend.DTOs;

public record CategoriaDto(int Id, string Nombre, string? Icono);

public record CrearCategoriaRequest(string Nombre, string? Icono);
