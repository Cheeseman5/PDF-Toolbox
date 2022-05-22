using System.Collections.Generic;
using System.Windows;
using System.IO;
using PDFToolbox.Interfaces.Helpers;

namespace PDFToolbox.Helpers
{
    public class FileDropExtractor : IFileIOExtractor
    {
        public Models.FileIOInfo[] GetFileStreams(IDataObject data)
        {
            var files = data?.GetData(DataFormats.FileDrop) as string[];

            if (files == null || files?.Length == 0)
            {
                return null;
            }

            var infos = new List<Models.FileIOInfo>();

            foreach (string file in files)
            {
                var info = GetFileInfo(file);
                infos.Add(info);
            }
            return infos.ToArray();
        }

        private Models.FileIOInfo GetFileInfo(string filePath)
        {
            var info = new Models.FileIOInfo();
            info.Stream = File.OpenRead(filePath);
            info.FullFileName = filePath;

            return info;
        }
    }
}
