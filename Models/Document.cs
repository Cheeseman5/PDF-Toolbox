using System.Collections.Generic;
using System.IO;

namespace PDFToolbox.Models
{
    public class Document : Page
    {
        public FileStream document { get; set; }
        //public List<ViewModels.PageViewModel> pages { get; set; }

        public Document()
        {
            //pages = new List<ViewModels.PageViewModel>();
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
    }
}
