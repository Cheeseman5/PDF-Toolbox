using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;

using iTextSharp.text.pdf;

namespace PDFToolbox.Models
{
    public class Page
    {
        // image used to display in pageDict/document views
        public BitmapImage Image;
        public string FileName;
        public int OriginalPageNumber;
        public PdfNumber Rotation;
        public PdfNumber OriginalRotation;
        // True = image is ignored on PDF creation; False = image is used
        public bool IsImagePreview;
        public Stream ImageStream;
        public ObservableCollection<UIString> UIStrings;
        public int ID;
    }
}
