namespace TP3_API.DTOs.Auth
{
    public class RegisterDto
    {
        public string NombreUsuario { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        // Opcional: Podríamos recibir el RolId o dejarlo por defecto como Cajero (ej: RolId = 2)
    }
}