using IntelliSoftAPIV2.Dtos.Cotizacion;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using IntelliSoftAPIV2.Dtos.Cotizacion;

namespace IntelliSoftAPIV2.Configuration
{
    public class PdfGeneratorService
    {
        public byte[] GenerarPdfCotizacion(CotizacionDto cotizacion)
        {
            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Encabezado
            document.Add(new Paragraph("Cotización AquaGrow")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(18)
                .SimulateBold());

            document.Add(new Paragraph($"Clave: {cotizacion.ClaveCotizacion}"));
            document.Add(new Paragraph($"Fecha: {cotizacion.FechaSolicitud:dd/MM/yyyy}"));
            document.Add(new Paragraph($"Hectáreas: {cotizacion.Hectareas}"));
            document.Add(new Paragraph($"ID Cliente: {cotizacion.UsuarioId}"));

            document.Add(new Paragraph("\nDetalles de insumos:"));

            var table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Nombre");
            table.AddHeaderCell("Cantidad");
            table.AddHeaderCell("Precio");
            table.AddHeaderCell("Subtotal");

            foreach (var item in cotizacion.Detalles)
            {
                table.AddCell(item.InsumoId.ToString());
                table.AddCell(item.NombreInsumo);
                table.AddCell(item.Cantidad.ToString());
                table.AddCell($"${item.PrecioPromedio:F2}");
                table.AddCell($"${item.Subtotal:F2}");
            }

            document.Add(table);

            document.Add(new Paragraph($"\nCostos de desarrollo: ${cotizacion.PrecioBase:F2}"));
            document.Add(new Paragraph($"Ganancia: ${cotizacion.Ganancia:F2}"));
            document.Add(new Paragraph($"Precio de venta final: ${cotizacion.PrecioConGanancia:F2}"));

            document.Close();
            return stream.ToArray();
        }
    }
}
