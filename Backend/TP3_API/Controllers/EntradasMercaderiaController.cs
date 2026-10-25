using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP3_API.Data;
using TP3_API.Models;

namespace TP3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")] // REQUISITO CLAVE: Solo Administrador
    public class EntradasMercaderiaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EntradasMercaderiaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarEntrada([FromBody] EntradaMercaderiaDto dto)
        {
            // Obtener el ID del Admin logueado desde el Token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int adminId = int.Parse(userIdClaim);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entrada = new EntradaMercaderia
                {
                    Fecha = DateTime.Now,
                    Proveedor = dto.Proveedor,
                    UsuarioId = adminId
                };

                decimal totalEntrada = 0;
                foreach (var item in dto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null) return BadRequest($"Producto con ID {item.ProductoId} no encontrado.");

                    // Actualizar Stock Actual de mercadería
                    producto.StockActual += item.Cantidad;

                    entrada.Detalles.Add(new EntradaMercaderiaDetalle
                    {
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        CostoUnitario = item.CostoUnitario
                    });
                }

                _context.EntradasMercaderia.Add(entrada);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Entrada de mercadería registrada y stock actualizado correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al registrar la entrada", error = ex.Message });
            }
        }
    }

    // DTOs auxiliares para la petición
    public class EntradaMercaderiaDto
    {
        public string Proveedor { get; set; } = string.Empty;
        public List<EntradaDetalleDto> Detalles { get; set; } = new();
    }

    public class EntradaDetalleDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
    }
}