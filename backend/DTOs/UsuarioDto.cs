namespace backend.DTOs;

public record UsuarioDto(string Correo, string Nombre, string Permisos, bool Activo);

public record CrearUsuarioRequest(string Nombre, string Correo, string Permisos);

public record EditarUsuarioRequest(string Nombre, string Permisos, bool Activo);
