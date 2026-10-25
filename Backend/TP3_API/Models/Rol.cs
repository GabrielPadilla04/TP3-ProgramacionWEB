using System.ComponentModel.DataAnnotations;

namespace TP3_API.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty; // Ej: Administrador, Cajero

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}