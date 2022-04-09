using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;


namespace PDFToolbox.Helpers
{
    public class OutlookAttachmentExtractor : BaseFileIOExtractor
    {
        public FileIO _fileIO;

        public OutlookAttachmentExtractor(FileIO fileIO)
            : base()
        {
            _fileIO = fileIO;
        }

        public override FileIOInfo[] GetFileStreams(IDataObject data)
        {
            if (!data.GetDataPresent("FileGroupDescriptor")) return null;

            string tmpPath;
            string fName;
            List<Models.Document> docs;
            Models.Document doc;
            Stream stream = data.GetData("FileGroupDescriptorW") as Stream;
            FileStream file;
            FileIOInfo info;

            if (stream != null)
            {

                fName = GetAttFileName(stream);
                tmpPath = _fileIO.SaveDirectoryTemp;
                docs = new List<Models.Document>();
                doc = new Models.Document();
                info = new FileIOInfo();


                if (data.GetDataPresent("FileContents", false))
                {
                    info.Stream = data.GetData("FileContents", true) as MemoryStream;
                    info.FullFileName = tmpPath + fName;
                    info.IsTempPath = true;

                    file = File.Create(info.FullFileName);
                    info.Stream.CopyTo(file);
                    file.Close();

                    return new FileIOInfo[] { info };
                }
            }
            return null;
        }

        private string GetAttFileName(Stream stream)
        {
            if (stream == null) return "";

            string fName = "";
            byte[] fileRaw = new byte[512];
            stream.Read(fileRaw, 0, 512);

            for (int i = 76; fileRaw[i] != 0; i+=2)
            {
                fName += Convert.ToChar(fileRaw[i]);
            }
            stream.Close();

            return fName;
        }
    }
}
