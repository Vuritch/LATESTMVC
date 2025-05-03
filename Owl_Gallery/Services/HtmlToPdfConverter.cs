using SelectPdf;

namespace Owl_Gallery.Services
{
    public class HtmlToPdfConverter
    {
        public byte[] ConvertHtmlToPdf(string html)
        {
            var converter = new SelectPdf.HtmlToPdf();
            var doc = converter.ConvertHtmlString(html);
            var pdfBytes = doc.Save();
            doc.Close();
            return pdfBytes;
        }
    }
}
