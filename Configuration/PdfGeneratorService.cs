using System.IO;
using IntelliSoftAPIV2.Dtos.Cotizacion;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace IntelliSoftAPIV2.Configuration
{

    public class PdfGeneratorService
    {
        // PDF para cliente: header + N partidas + insumos (solo cantidad) + precio final por partida + TOTAL final
        public byte[] GenerarPdfCotizacionConPartidas(CotizacionFullDto cot)
        {
            using var stream = new MemoryStream();
            using (var writer = new PdfWriter(stream))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                // Encabezado
                document.Add(new Paragraph("Cotización AquaGrow")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18));

                if (!string.IsNullOrWhiteSpace(cot.ClaveCotizacion))
                    document.Add(new Paragraph($"Clave: {cot.ClaveCotizacion}"));

                if (cot.FechaSolicitud.HasValue)
                    document.Add(new Paragraph($"Fecha: {cot.FechaSolicitud:dd/MM/yyyy}"));

                if (!string.IsNullOrWhiteSpace(cot.NombreCliente) || !string.IsNullOrWhiteSpace(cot.UsuarioId))
                    document.Add(new Paragraph($"Cliente: {cot.NombreCliente ?? cot.UsuarioId}"));

                if (!string.IsNullOrWhiteSpace(cot.DetalleCotizacion))
                    document.Add(new Paragraph($"Detalle: {cot.DetalleCotizacion}"));

                document.Add(new Paragraph("\nPartidas:\n"));

                decimal totalVentaCotizacion = 0m; // suma de (PrecioBase + Ganancia) por partida
                int idx = 1;

                foreach (var p in cot.Partidas)
                {
                    // Título de la partida
                    document.Add(new Paragraph($"#{idx} - {p.NombreProducto ?? ("Producto " + p.ProductoId)}")
                        .SetFontSize(13));

                    // Info breve
                    document.Add(new Paragraph($"Hectáreas: {p.Hectareas:0.##}"));

                    // Tabla de insumos: SOLO nombre y cantidad
                    var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2 }))
                        .UseAllAvailableWidth();

                    table.AddHeaderCell("Insumo");
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Cantidad"))
                        .SetTextAlignment(TextAlignment.RIGHT));

                    foreach (var d in p.Detalles)
                    {
                        table.AddCell(d.NombreInsumo ?? "");
                        table.AddCell(new Cell().Add(new Paragraph(d.Cantidad.ToString("0.##")))
                            .SetTextAlignment(TextAlignment.RIGHT));
                    }

                    document.Add(table);

                    // Precio de venta final de la partida = PrecioBase + Ganancia
                    var precioFinalPartida = decimal.Round(p.PrecioBase + p.Ganancia, 2);
                    totalVentaCotizacion += precioFinalPartida;

                    document.Add(new Paragraph($"\nPrecio de venta final de la partida: ${precioFinalPartida:F2}")
                        .SetFontSize(12)
                        .SetMarginBottom(15));

                    idx++;
                }

                // TOTAL de venta de toda la cotización
                totalVentaCotizacion = decimal.Round(totalVentaCotizacion, 2);
                document.Add(new Paragraph("\nTotal de venta de la cotización:")
                    .SetFontSize(13)
                    .SetFontColor(ColorConstants.BLACK));
                document.Add(new Paragraph($"${totalVentaCotizacion:F2}")
                    .SetFontSize(12));
            }

            return stream.ToArray();
        }
    }
}
