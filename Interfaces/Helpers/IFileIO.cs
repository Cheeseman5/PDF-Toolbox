using PDFToolbox.Models;
using System.Windows;

namespace PDFToolbox.Interfaces.Helpers
{
    public interface IFileIO
    {
        //void RegisterStrategy(BaseIOStrategy strategy);
        FileIOInfo[] ExtractFileInfo(IDataObject data);
        Document[] ExtractDocument(FileIOInfo[] files);
        Document[] ExtractDocument(IDataObject data);
        Document LoadDocument(FileIOInfo info);
        string ToTempFileName(string fPath);
        string ToTempFileName(string fPath, int pageNumber);
        bool IsFileValid(string fPath, bool checkFileExistance = false);
        bool IsExtensionSupported(string file);
    }
}
