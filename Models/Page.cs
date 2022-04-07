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
        // Original document file name
        public string fName { get; set; }
        // Original pageDict number
        public int number { get; set; }
        // Rotation of pageDict (in degrees)
        public PdfNumber rotation { get; set; }
        // Original rotation of pageDict (in degrees)
        public PdfNumber originalRotation { get; set; }
        // True = image is ignored on PDF creation; False = image is used
        public bool isImagePreview { get; set; }

        public Stream imageStream { get; set; }

        public ObservableCollection<UIString> uiStrings { get; set; }

        private static int nextID = 0;
        public int id { get; private set; }

        public Page()
        {
            id = nextID++;
            rotation = new PdfNumber(0f);
            originalRotation = new PdfNumber(0f);
        }

        public void Copy(Page page)
        {
            if (page == null) return;

            this.image = page.image;
            this.fName = page.fName;
            this.number = page.number;
            this.rotation = page.rotation;
            this.originalRotation = page.originalRotation;
            this.isImagePreview = page.isImagePreview;
            this.imageStream = page.imageStream;
            this.uiStrings = page.uiStrings;
        }

        public static Page MakeCopy(Page page)
        {
            Page p = new Page();

            p.Copy(page);

            return p;
        }

        public void SetRotation(float rotation)
        {
            this.rotation = new PdfNumber(rotation);
        }
    }
}
