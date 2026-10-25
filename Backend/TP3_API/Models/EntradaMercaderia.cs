using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP3_API.Models
{
    public class EntradaMercaderia
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        [MaxLength(150)]
        public string Proveedor { get; set; } = string.Empty;

        public int UsuarioId { get; set; } // Administrador que ingresa mercadería
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public ICollection<EntradaMercaderiaDetalle> Detalles { get; set; } = new List<EntradaMercaderiaDetalle>();
    }
}