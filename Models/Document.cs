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
        public BitmapImage image { get; set; }
        public string fName { get; set; } = "";
        public int originalPageNumber { get; set; }
        public PdfNumber rotation { get; set; } = new PdfNumber(0f);
        // Original rotation of pageDict (in degrees)
        public PdfNumber originalRotation { get; set; } = new PdfNumber(0f);
        // True = image is ignored on PDF creation; False = image is used
        public bool isImagePreview { get; set; }
        public Stream imageStream { get; set; }
        public ObservableCollection<UIString> uiStrings { get; set; }
        public int ID { get; set; }

        public FileStream document { get; set; }
        public List<PageViewModel> pages { get; set; } = new List<PageViewModel>();

        public class PageViewModel
        {
            private Page _page;
            public BitmapImage Image { get; set; }
            public string DocName { get; set; }
            public double Scale { get; set; }
            public bool IsSelected { get; set; }
            public int Number { get; set; }
            public float Rotation { get; set; }
            public float FlatRotation { get; set; }
            public Stream ImageStream { get; set; }
            public ObservableCollection<Models.UIString> Strings { get; set; }
            public int ID { get; set; }
            public void OnStringsChanged(object sender, NotifyCollectionChangedEventArgs e) { }
            public PageViewModel(Page page)
            { _page = page; }
            public void Copy(PageViewModel page) { }

        }
    }
}
