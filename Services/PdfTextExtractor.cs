using UglyToad.PdfPig;
using System.Text;
using System.IO;

namespace ResumeMatcherAPI.Services;

public class PdfTextExtractor
{
    public string ExtractText(Stream pdfStream)
    {
        var text = new StringBuilder();

        using var document = PdfDocument.Open(pdfStream);
        foreach (var page in document.GetPages())
        {
            text.AppendLine(page.Text);
        }

        return text.ToString();
    }
}
