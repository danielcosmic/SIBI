namespace backend.DTOs;

public record EncargadoDto(Guid Id, string Nombre, string Rol);
public record CrearEncargadoRequest(string Nombre, string Rol);
public record EditarEncargadoRequest(string Nombre, string Rol);
public record ReasignarActivosRequest(List<string> Placas, Guid NuevoEncargadoId);
