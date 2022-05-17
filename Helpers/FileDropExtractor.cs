using System.Collections.Generic;
using System.Windows;
using System.IO;
using PDFToolbox.Interfaces.Helpers;

namespace PDFToolbox.Helpers
{
    public class FileDropExtractor : IFileIOExtractor
    {
        public FileDropExtractor()
            : base()
        { }

        public Models.FileIOInfo[] GetFileStreams(IDataObject data)
        {
            if(data==null || !data.GetDataPresent(DataFormats.FileDrop)) return null;

            List<Models.FileIOInfo> infos;
            Models.FileIOInfo info;
            string[] files = data.GetData(DataFormats.FileDrop) as string[];

            if (files != null && files.Length > 0)
            {
                infos = new List<Models.FileIOInfo>();

                foreach (string file in files)
                {
                    info = new Models.FileIOInfo();
                    info.Stream = File.OpenRead(file);
                    info.FullFileName = file;
                    
                    infos.Add(info);
                }
                return infos.ToArray();
            }
            return null;
        }
    }
}
