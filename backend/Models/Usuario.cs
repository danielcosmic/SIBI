namespace backend.Models;

public class Usuario
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string Permisos { get; set; } = string.Empty; // Administradora | GTI | JefaAdministrativa | Invitado
    public bool EsContrasenaTemporal { get; set; }
    public int IntentosFallidos { get; set; }
    public bool Activo { get; set; } = true;
}
