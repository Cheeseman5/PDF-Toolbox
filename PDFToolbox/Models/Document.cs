using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;

namespace PDFToolbox.Models
{
    public class Document : Page
    {
        public FileStream document { get; set; }
        //public PdfiumViewer.PdfDocument pDoc { get; set; }
        public List<ViewModels.PageViewModel> pages { get; set; }

        public Document()
        {
            pages = new List<ViewModels.PageViewModel>();
        }

        /*public bool LoadFile(string fPath)
        {
            List<Models.Page> pageModels = Toolbox.LoadDocument(fPath);

            if (pageModels == null || pageModels.Count == 0)
            {
                Toolbox.MessageBox("Need something to save...");
                return false;
            }

            image = pageModels[0].image;
            fName = pageModels[0].fName;

            for (int i = 0; i < pageModels.Count; i++)
            {
                pages.Add(new ViewModels.PageViewModel(pageModels[i]));
            }
            return true;
        }*/
    }
}
