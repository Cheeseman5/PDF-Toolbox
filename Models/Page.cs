using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;

using iTextSharp.text.pdf;

namespace PDFToolbox.Models
{
    public class Page
    {
        // image used to display in pageDict/document views
        public BitmapImage image { get; set; }
        public string fName { get; set; }
        // Original page number
        public int number { get; set; }
        public PdfNumber rotation { get; set; } = new PdfNumber(0f);
        // Original rotation of pageDict (in degrees)
        public PdfNumber originalRotation { get; set; } = new PdfNumber(0f);
        // True = image is ignored on PDF creation; False = image is used
        public bool isImagePreview { get; set; }

        public Stream imageStream { get; set; }

        public ObservableCollection<UIString> uiStrings { get; set; }
        public int ID { get; set; }
    }
}
