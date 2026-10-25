using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP3_API.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string CodigoBarras { get; set; } = string.Empty;
        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }

        public int StockActual { get; set; }
        public int StockMinimo { get; set; }

        public ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
        public ICollection<EntradaMercaderiaDetalle> EntradaDetalles { get; set; } = new List<EntradaMercaderiaDetalle>();
    }
}