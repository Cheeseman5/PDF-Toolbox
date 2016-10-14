using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace PDFToolbox.IO
{
    public abstract class BaseFileIOExtractor : BaseIOStrategy
    {
        public BaseFileIOExtractor()
            : base()
        { }

        public abstract FileIOInfo[] GetFileStreams(IDataObject data);

    }
}
