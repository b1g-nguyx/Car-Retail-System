using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;

namespace Car_Rental_System.Services;
public interface IPdfService
{
    Task<byte[]> GeneratePdfFromHtml(string htmlContent);
}

public class PdfService
{
    private readonly IConverter _converter;

    public PdfService(IConverter converter)
    {
        _converter = converter;
    }
    public async Task<byte[]> GeneratePdfFromHtml(string htmlContent)
    {
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = new GlobalSettings()
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
            },
            Objects = {
        new ObjectSettings()
        {
            HtmlContent = htmlContent,
            WebSettings = { DefaultEncoding = "utf-8", LoadImages = true}
        }
    }
        };
var result = _converter.Convert(doc);

        return result;
    }

}