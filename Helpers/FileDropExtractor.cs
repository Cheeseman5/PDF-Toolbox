using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace PDFToolbox.Helpers
{
    public class FileDropExtractor : BaseFileIOExtractor
    {
        public FileDropExtractor()
            : base()
        { }

        public override FileIOInfo[] GetFileStreams(IDataObject data)
        {
            if(data==null || !data.GetDataPresent(DataFormats.FileDrop)) return null;

            List<FileIOInfo> infos;
            FileIOInfo info;
            string[] files = data.GetData(DataFormats.FileDrop) as string[];

            if (files != null && files.Length > 0)
            {
                infos = new List<FileIOInfo>();

                foreach (string file in files)
                {
                    info = new FileIOInfo();
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
