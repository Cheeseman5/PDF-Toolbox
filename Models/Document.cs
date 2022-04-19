using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Media.Imaging;

namespace PDFToolbox.Models
{
    public class Document
    {
        // image used to display in pageDict/document views
        public BitmapImage Image;
        public string FileName;
        public int OriginalPageNumber;
        public PdfNumber Rotation;
        // Original rotation of pageDict (in degrees)
        public PdfNumber OriginalRotation;
        // True = image is ignored on PDF creation; False = image is used
        public bool IsImagePreview;
        public Stream ImageStream;
        public ObservableCollection<UIString> UIStrings;
        public int ID;
        public FileStream DocumentFile;
        public List<Page> Pages;
    }
}
