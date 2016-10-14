using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFToolbox.Common
{
    public class UIString //: IDisposable
    {
        private string _string = "";
        public string String
        {
            get { return _string; }
            set { _string = value;}
        }

        private double _x = 0;
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        private double _y = 0;
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        private double _width = 0;
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private double _height = 0;
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
