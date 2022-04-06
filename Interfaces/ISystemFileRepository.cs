using System.IO;

namespace PDFToolbox.Interfaces
{
    public interface ISystemFileRepository
    {
        MemoryStream Load(string fPath);
        //Models.Document LoadDocument(IO.FileIOInfo info);
        //void SaveDocument(ViewModels.DocumentViewModel document);
    }
}
