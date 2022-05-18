using PDFToolbox.Models;
using System.Windows;

namespace PDFToolbox.Interfaces.Helpers
{
    public interface IFileIO
    {
        void RegisterStrategy(IFileIOStrategy strategy);
        void RegisterExtractor(IFileIOExtractor extractor);
        FileIOInfo[] ExtractFileInfo(IDataObject data);
        Document[] ExtractDocument(FileIOInfo[] files);
        Document[] ExtractDocument(IDataObject data);
        Document LoadDocument(FileIOInfo info);
        string ToTempFileName(string fPath);
        string ToTempFileName(string fPath, int pageNumber);
        bool IsFileValid(string fPath, bool checkFileExistance = false);
        bool IsExtensionSupported(string file);

        void CopyToTemp(string fPath);
        string MakeFilePathSafe(string fPath, string defaultSaveDirectory);
        void CreateDirectory(string directoryPath);
    }
}
