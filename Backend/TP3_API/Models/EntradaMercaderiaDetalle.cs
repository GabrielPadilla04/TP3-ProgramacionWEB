using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP3_API.Models
{
    public class EntradaMercaderiaDetalle
    {
        [Key]
        public int Id { get; set; }

        public int EntradaId { get; set; }
        [ForeignKey("EntradaId")]
        public EntradaMercaderia? EntradaMercaderia { get; set; }

        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }
    }
}