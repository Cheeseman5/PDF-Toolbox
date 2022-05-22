using PDFToolbox.Models;

namespace PDFToolbox.Interfaces.Helpers
{
    public interface IFileIOStrategy
    {
        string[] SupportedExtentions { get; set; }
        Document LoadDocument(FileIOInfo info);
        //void SaveDocument(ViewModels.DocumentViewModel document);
        string CopyToTemp(string fPath);
        void SetSupportedExtensions(params string[] extensions);
        bool IsExtensionSupported(string extension);
        bool IsFileSupported(string fPath);
        string SafeFilePath(string fPath);
    }
}
