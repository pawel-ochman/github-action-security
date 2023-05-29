using iText.Html2pdf;
using iText.Kernel.Pdf;
using OneOf.Types;
using System.Text;

namespace GithubActionsDotnet.Security.Services;

public interface IPdfGeneration
{
    void GeneratePdfFromHtml(string html, string outputFile);
}

public class PdfGeneration : IPdfGeneration
{ 
    public void GeneratePdfFromHtml(string html, string outputFile)
    {
        ConverterProperties properties = new ConverterProperties();

        PdfWriter writer = new PdfWriter(outputFile);

        PdfDocument pdf = new PdfDocument(writer);
        properties.SetBaseUri("https://google.com");

        HtmlConverter.ConvertToPdf(new MemoryStream(Encoding.UTF8.GetBytes(html)), pdf, properties);
    }
}