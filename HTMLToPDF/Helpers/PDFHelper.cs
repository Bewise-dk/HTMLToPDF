using HTMLToPDF.Models;
using System;

namespace HTMLToPDF.Helpers
{
    public class PDFHelper : IPDFHelper
    {
        public string ModifyHTML(PDFHandler.PDFInput pdfInput)
        {
            string HTML = pdfInput.HTML;
            string ColorCode = pdfInput.ColorCode;
            string FontName = pdfInput.FontName;

            var customColor = !string.IsNullOrEmpty(ColorCode) ? "* { color: #" + ColorCode + "; }" : "";
            HTML = $"<html><head><style type=\"text/css\">@page {{margin: 0;}} {customColor} body {{ font-family: {FontName} }} * {{webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;}} </style><meta charset=\"UTF-8\"></head><body>{HTML}</body></html>";

            return HTML;
        }
        public float ConvertPixelsToPoints(string input)
        {
            return (float)Math.Ceiling((double.Parse(input) * 0.75));
        }
    }
}