using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP3_API.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public int RolId { get; set; }
        [ForeignKey("RolId")]
        public Rol? Rol { get; set; }

        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
        public ICollection<EntradaMercaderia> EntradasMercaderia { get; set; } = new List<EntradaMercaderia>();
    }
}