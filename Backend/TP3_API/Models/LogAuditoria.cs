using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP3_API.Models
{
    public class LogAuditoria
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        [MaxLength(50)]
        public string Nivel { get; set; } = string.Empty; // Info, Error, Warning
        public string Mensaje { get; set; } = string.Empty;
        public string? StackTrace { get; set; }

        public int? UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
    }
}