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
                    page.Margin(35);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    // --- ENCABEZADO ---
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("SISTEMA POS - REPORTE GENERAL DEL NEGOCIO").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                            col.Item().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    // --- CONTENIDO ---
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // 1. ESTADO ACTUAL DE STOCK
                        col.Item().Text("1. Estado Actual de Stock").Bold().FontSize(11).FontColor(Colors.Blue.Medium);
                        col.Item().PaddingBottom(5);

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
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(4).Text("ID").Bold();
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(4).Text("Producto").Bold();
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(4).Text("Stock").Bold();
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(4).Text("Precio").Bold();
                            });

                            foreach (var p in productos)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.Id.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.Nombre);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(p.StockActual.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"${p.PrecioVenta:N2}");
                            }
                        });

                        col.Item().PaddingBottom(20);

                        // 2. HISTORIAL DE ENTRADAS DE MERCADERÍA
                        col.Item().Text("2. Historial de Entradas de Mercadería").Bold().FontSize(11).FontColor(Colors.Green.Medium);
                        col.Item().PaddingBottom(5);

                        if (!entradas.Any())
                        {
                            col.Item().Text("No hay entradas registradas.").Italic().FontSize(9);
                        }
                        else
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(2.5f);
                                    columns.RelativeColumn(3f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Green.Lighten4).Padding(4).Text("ID").Bold();
                                    header.Cell().Background(Colors.Green.Lighten4).Padding(4).Text("Fecha").Bold();
                                    header.Cell().Background(Colors.Green.Lighten4).Padding(4).Text("Proveedor").Bold();
                                    header.Cell().Background(Colors.Green.Lighten4).Padding(4).Text("Productos Ingresados").Bold();
                                });

                                foreach (var ent in entradas)
                                {
                                    string detalleItems = string.Join(", ", ent.Detalles.Select(d => $"{d.Cantidad}x {d.Producto?.Nombre ?? "Prod."}"));

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ent.Id.ToString());
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ent.Fecha.ToString("dd/MM/yyyy"));
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ent.Proveedor);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(detalleItems);
                                }
                            });
                        }

                        col.Item().PaddingBottom(20);

                        // 3. HISTORIAL DE VENTAS
                        col.Item().Text("3. Historial de Ventas").Bold().FontSize(11).FontColor(Colors.Orange.Medium);
                        col.Item().PaddingBottom(5);

                        if (!ventas.Any())
                        {
                            col.Item().Text("No hay ventas registradas.").Italic().FontSize(9);
                        }
                        else
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(3f);
                                    columns.RelativeColumn(1.2f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Orange.Lighten4).Padding(4).Text("ID").Bold();
                                    header.Cell().Background(Colors.Orange.Lighten4).Padding(4).Text("Fecha").Bold();
                                    header.Cell().Background(Colors.Orange.Lighten4).Padding(4).Text("Detalle de Artículos").Bold();
                                    header.Cell().Background(Colors.Orange.Lighten4).Padding(4).Text("Total").Bold();
                                });

                                foreach (var v in ventas)
                                {
                                    string detalleVenta = string.Join(", ", v.Detalles.Select(d => $"{d.Cantidad}x {d.Producto?.Nombre ?? "Prod."}"));

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.Id.ToString());
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.Fecha.ToString("dd/MM/yyyy HH:mm"));
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(detalleVenta);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"${v.Total:N2}");
                                }
                            });
                        }
                    });

                    // --- PIE DE PÁGINA ---
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