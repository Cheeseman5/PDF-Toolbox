using PDFToolbox.Models;
using System.Windows;

namespace PDFToolbox.Interfaces.Helpers
{
    public interface IFileIOExtractor
    {
        FileIOInfo[] GetFileStreams(IDataObject data);
    }
}
