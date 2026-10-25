using Microsoft.EntityFrameworkCore;
using TP3_API.Models;

namespace TP3_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        public DbSet<EntradaMercaderia> EntradasMercaderia { get; set; }
        public DbSet<EntradaMercaderiaDetalle> EntradaMercaderiaDetalles { get; set; }
        public DbSet<LogAuditoria> LogsAuditoria { get; set; }
    }
}