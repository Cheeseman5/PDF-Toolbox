using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PDFToolbox.PDF
{
    public class PdfItem<T>
    {
        private T _item;
        public T Item
        {
            get { return _item; }
            set { _item = value; }
        }

        private Point _pagePosition;
        public Point Position
        {
            get { return _pagePosition; }
            set { _pagePosition = value; }
        }



    }
}
