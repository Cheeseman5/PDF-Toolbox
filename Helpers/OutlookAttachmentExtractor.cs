using PDFToolbox.Interfaces;
using PDFToolbox.Interfaces.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;


namespace PDFToolbox.Helpers
{
    public class OutlookAttachmentExtractor : IFileIOExtractor
    {
        private IConfig _config;

        public OutlookAttachmentExtractor(IConfig config)
        {
            _config = config;
        }

        public Models.FileIOInfo[] GetFileStreams(IDataObject data)
        {
            var stream = data?.GetData("FileGroupDescriptor") as Stream;

            if (stream == null || !data.GetDataPresent("FileContents", false))
            {
                return null;
            }

            string fileName = GetAttachmentFileName(stream);
            string tmpPath = _config.DefaultTempSaveDirectory;

            Models.FileIOInfo info = GetFileData(data, tmpPath, fileName);

            return new Models.FileIOInfo[] { info };
        }

        private Models.FileIOInfo GetFileData(IDataObject data, string tempPath, string fileName)
        {
            var info = new Models.FileIOInfo();

            info.Stream = data.GetData("FileContents", true) as MemoryStream;
            info.FullFileName = tempPath + fileName;
            info.IsTempPath = true;

            using (FileStream file = File.Create(info.FullFileName))
            {
                info.Stream.CopyTo(file);
            }

            return info;
        }

        private string GetAttachmentFileName(Stream stream)
        {
            if (stream == null)
            {
                return "";
            }

            string fName = "";
            byte[] fileRaw = new byte[512]; // why 512...?
            stream.Read(fileRaw, 0, 512);

            // why 76...?
            for (int i = 76; fileRaw[i] != 0; i+=2)
            {
                fName += Convert.ToChar(fileRaw[i]);
            }
            stream.Close();

            return fName;
        }
    }
}
