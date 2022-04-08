using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Media.Imaging;

namespace PDFToolbox.Models
{
    public class Document : Page
    {
        public FileStream document { get; set; }
        //public List<PageViewModel> pages { get; set; }

        public List<PageViewModel> pages { get; set; }

        public Document()
        {
            pages = new List<PageViewModel>();
            fName = "";
        }

        public void Rename(string newName, bool append=false)
        {
            try
            {
                newName = (append) ? Path.GetFileNameWithoutExtension(fName) + newName : newName;

                fName = Path.GetDirectoryName(fName) + "\\" + newName + Path.GetExtension(fName);
            }
            catch
            {
                // may be a bad idea...
                fName = newName;
            }
        }

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
