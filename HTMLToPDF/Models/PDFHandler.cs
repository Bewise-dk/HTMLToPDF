
namespace HTMLToPDF.Models
{
    public class PDFHandler
    {
        public class PDFInput
        {
            public string FontFileName { get; set; }
            public string FontName { get; set; }
            public string HTML { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Base64 { get; set; }
            public bool Finalize { get; set; }
            public string ColorCode { get; set; }
            public string PageWidth { get; set; }
            public string PageHeight { get; set; }
            public string Currency { get; set; }
            public int Type { get; set; }
            public int LanguageID { get; set; }
            public string Number { get; set; }
            public bool FontLoaded { get; set; }
        }
        public class PDFCustomer
        {
            public string ID { get; set; }
            public string Base64 { get; set; }
        }
    }
}