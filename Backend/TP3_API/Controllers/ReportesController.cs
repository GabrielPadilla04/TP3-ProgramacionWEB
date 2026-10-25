using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TP3_API.Data;

namespace TP3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [HttpGet("general-pdf")]
        public async Task<IActionResult> DescargarReporteGeneral()
        {
            var productos = await _context.Productos.ToListAsync();
            var entradas = await _context.EntradasMercaderia
                .Include(e => e.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40); // Margen en puntos (seguro y compatible)
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("SISTEMA POS - REPORTE GENERAL DEL NEGOCIO").Bold().FontSize(16).FontColor(Colors.Blue.Darken2);
                            col.Item().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // --- SECCIÓN 1: STOCK ACTUAL ---
                        col.Item().Text("1. Estado Actual de Stock").Bold().FontSize(12).FontColor(Colors.Blue.Medium);
                        col.Item().PaddingBottom(10);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(3).Text("ID").Bold();
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(3).Text("Producto").Bold();
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(3).Text("Stock").Bold();
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(3).Text("Precio").Bold();
                            });

                            foreach (var p in productos)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(p.Id.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(p.Nombre);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(p.StockActual.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text($"${p.PrecioVenta:N2}");
                            }
                        });

                        col.Item().PaddingBottom(20);

                        // --- SECCIÓN 2: ENTRADAS DE MERCADERÍA ---
                        col.Item().Text("2. Historial de Entradas de Mercadería").Bold().FontSize(12).FontColor(Colors.Green.Medium);
                        col.Item().PaddingBottom(10);

                        if (!entradas.Any())
                        {
                            col.Item().Text("No hay entradas registradas.").Italic().FontSize(9);
                        }
                        else
                        {
                            foreach (var ent in entradas)
                            {
                                col.Item().Background(Colors.Grey.Lighten3).Padding(4).Text($"Entrada #{ent.Id} - Proveedor: {ent.Proveedor} ({ent.Fecha:dd/MM/yyyy})").Bold().FontSize(9);
                                foreach (var det in ent.Detalles)
                                {
                                    col.Item().PaddingLeft(10).Text($"- {det.Producto?.Nombre}: {det.Cantidad} unidades (Costo U: ${det.CostoUnitario:N2})");
                                }
                                col.Item().PaddingBottom(5);
                            }
                        }

                        col.Item().PaddingBottom(20);

                        // --- SECCIÓN 3: VENTAS ---
                        col.Item().Text("3. Historial de Ventas").Bold().FontSize(12).FontColor(Colors.Orange.Medium);
                        col.Item().PaddingBottom(10);

                        if (!ventas.Any())
                        {
                            col.Item().Text("No hay ventas registradas.").Italic().FontSize(9);
                        }
                        else
                        {
                            foreach (var v in ventas)
                            {
                                col.Item().Background(Colors.Grey.Lighten3).Padding(4).Text($"Venta #{v.Id} - Fecha: {v.Fecha:dd/MM/yyyy} - Total: ${v.Total:N2}").Bold().FontSize(9);
                                foreach (var det in v.Detalles)
                                {
                                    col.Item().PaddingLeft(10).Text($"- {det.Producto?.Nombre}: {det.Cantidad} un. x ${det.PrecioUnitario:N2}");
                                }
                                col.Item().PaddingBottom(5);
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            var pdfBytes = documento.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Reporte_General_Sistema_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
    }
}