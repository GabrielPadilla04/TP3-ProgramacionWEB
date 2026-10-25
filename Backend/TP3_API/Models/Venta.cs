using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP3_API.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public int UsuarioId { get; set; } // Cajero autenticado
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}