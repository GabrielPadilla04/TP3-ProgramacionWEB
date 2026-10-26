using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP3_API.Data;
using TP3_API.Models;

namespace TP3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos (Público: Cualquiera puede ver el catálogo en el POS)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        
        [HttpGet("buscar")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductos([FromQuery] string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return Ok(new List<Producto>());

            var productos = await _context.Productos
                .Where(p => p.Nombre.Contains(filtro) || p.CodigoBarras.Contains(filtro))
                .Take(15) // Limitamos a 15 para que la búsqueda sea rápida
                .ToListAsync();

            return Ok(productos);
        }

        // POST: api/Productos (Protegido: Solo el Administrador puede ingresar mercadería)
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
        }
    }
}