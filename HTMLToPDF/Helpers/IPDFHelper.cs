using HTMLToPDF.Models;

namespace HTMLToPDF.Helpers
{
    public interface IPDFHelper
    {
        string ModifyHTML(PDFHandler.PDFInput pdfInput);
        float ConvertPixelsToPoints(string input);
    }
}