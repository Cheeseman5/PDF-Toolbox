using System.Windows;

namespace PDFToolbox.Helpers
{
    public abstract class BaseFileIOExtractor : BaseIOStrategy
    {
        public BaseFileIOExtractor()
            : base()
        { }

        public abstract FileIOInfo[] GetFileStreams(IDataObject data);

    }
}
