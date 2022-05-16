using Ghostscript.NET.Processor;
using HTMLToPDF.Helpers;
using HTMLToPDF.Models;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace HTMLToPDF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IPDFHelper pdfHelper;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, IPDFHelper pdfHelper)
        {
            _logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            this.pdfHelper = pdfHelper;
        }

        public IActionResult Index()
        {
            return Content("");
        }


        [Route("CreatePDF")]
        [HttpPost]
        public async Task<IActionResult> CreatePDF([FromBody] PDFHandler.PDFInput pdfInput)
        {
            var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            string wwwRoot = hostingEnvironment.WebRootPath;
            var customerID = Guid.NewGuid().ToString();
            //string base64 = "";

            while (System.IO.File.Exists($"{wwwRoot}/pdfs/{customerID}.pdf"))
            {
                customerID = Guid.NewGuid().ToString();
            }

            var fileName = $"{customerID}.pdf";

            var TempPath = $"{wwwRoot}\\pdfs\\temp_{fileName}";
            var Path = $"{wwwRoot}\\pdfs\\{fileName}";
            string FONT = hostingEnvironment.WebRootPath + $"\\fonts\\{pdfInput.FontFileName}.woff";

            double PixelInCm = 37.7952755906;

            pdfInput.PageWidth = Math.Round((double.Parse(pdfInput.PageWidth.Replace(".", ",").Replace("cm", "")) * PixelInCm), 2).ToString();
            pdfInput.PageHeight = Math.Round((double.Parse(pdfInput.PageHeight.Replace(".", ",").Replace("cm", "")) * PixelInCm), 2).ToString();

            PageSize pageSize = new PageSize(pdfHelper.ConvertPixelsToPoints(pdfInput.PageWidth), pdfHelper.ConvertPixelsToPoints(pdfInput.PageHeight));

            pdfInput.HTML = pdfHelper.ModifyHTML(pdfInput);

            if (pdfInput.Finalize)
            {
                ConverterProperties properties = new ConverterProperties();
                FontProvider fontProvider = new DefaultFontProvider(false, false, false);
                try
                {
                    FontProgram fontProgram = FontProgramFactory.CreateFont(FONT);
                    fontProvider.AddFont(fontProgram, "Winansi");
                }
                catch (Exception ex)
                {
                    var a = ex.Message;
                }
                properties.SetFontProvider(fontProvider);

                var pdfWriter = new PdfWriter(TempPath);

                PdfDocument pdf = new PdfDocument(pdfWriter);
                pdf.SetDefaultPageSize(pageSize);

                try
                {
                    HtmlConverter.ConvertToPdf(pdfInput.HTML, pdf, properties);

                    using (GhostscriptProcessor ghostscript = new GhostscriptProcessor())
                    {
                        List<string> switches = new List<string>();
                        switches.Add("-dPDFA");
                        switches.Add("-dBATCH");
                        switches.Add("-dNOPAUSE");
                        switches.Add("-dNoOutputFonts");
                        switches.Add("-dUseCIEColor");
                        switches.Add("-sProcessColorModel=DeviceCMYK");
                        switches.Add("-sDEVICE=pdfwrite");
                        switches.Add("-sPDFACompatibilityPolicy=1");
                        switches.Add(@"-sOutputFile=" + Path);
                        switches.Add(@"-f");
                        switches.Add(TempPath);

                        ghostscript.Process(switches.ToArray());

                        System.IO.File.Delete(TempPath);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                var bytes = Convert.FromBase64String(pdfInput.Base64);

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    Bitmap image = new Bitmap(memoryStream);
                    int width = image.Width;
                    int height = image.Height;

                    int maxWidth = 350;
                    int maxHeight = 350;

                    decimal rnd = Math.Min(maxWidth / (decimal)width, maxHeight / (decimal)height);

                    Image resizedImage = new Bitmap(image, new Size((int)Math.Round(width * rnd), (int)Math.Round(height * rnd)));

                    resizedImage.Save($"{wwwRoot}/pdfs/{customerID}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    resizedImage.Save($"{wwwRoot}/pdfs/{customerID}-t.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                var customerData = new PDFHandler.PDFCustomer() { Base64 = "", ID = customerID };

                return Json(customerData);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}