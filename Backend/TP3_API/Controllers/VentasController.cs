using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP3_API.Data;
using TP3_API.Models;

namespace TP3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cajero,Cliente,Administrador")]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VentaDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int usuarioId = int.Parse(userIdClaim);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    UsuarioId = usuarioId,
                    Total = 0
                };

                decimal totalVenta = 0;

                foreach (var item in dto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null) return BadRequest($"Producto {item.ProductoId} no encontrado.");

                    if (producto.StockActual < item.Cantidad)
                        return BadRequest(new { mensaje = $"Stock insuficiente para el producto: {producto.Nombre}. Stock actual: {producto.StockActual}" });

                    // Descontar Stock
                    producto.StockActual -= item.Cantidad;

                    var subtotal = item.Cantidad * producto.PrecioVenta;
                    totalVenta += subtotal;

                    venta.Detalles.Add(new VentaDetalle
                    {
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.PrecioVenta,
                        Subtotal = subtotal
                    });
                }

                venta.Total = totalVenta;
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Venta registrada con éxito", ventaId = venta.Id, total = totalVenta });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al procesar la venta", error = ex.Message });
            }
        }
    }

    public class VentaDto
    {
        public List<VentaDetalleDto> Detalles { get; set; } = new();
    }

    public class VentaDetalleDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}