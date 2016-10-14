using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
//using System.Windows.Media;

namespace PDFToolbox.PDF
{
    public class PdfData
    {
        public float Rotation { get; set; } // value % 360f
        public PdfItem<Bitmap> Image { get; set; }

        public PdfData()
        { }

    }
}
