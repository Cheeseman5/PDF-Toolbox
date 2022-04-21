using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PDFToolbox.Helpers
{
    public abstract class BaseFileIOStrategy : BaseIOStrategy
    {
        protected Toolbox _toolbox;
        protected FileIO _fileIO;
        public string[] SupportedExtentions { get; protected set; }

        private List<string> _tmpFiles;

        public BaseFileIOStrategy(Toolbox toolbox, FileIO fileIO)
            : base()
        {
            _toolbox = toolbox;
            _fileIO = fileIO;
            _tmpFiles = new List<string>();
        }

        //public virtual MemoryStream Load(string fPath)
        //{
        //    if (string.IsNullOrEmpty(fPath)) throw new ArgumentNullException("fPath","fPath was null or empty");
        //    if(!IsFileSupported(fPath)) throw new FileLoadException("File type is not supported by this object",fPath);

        //    MemoryStream stream = null;
        //    string fullPath = Path.GetFullPath(fPath);
        //    FileStream file = null;

        //    if (File.Exists(fullPath))
        //    {
        //        try
        //        {
        //            file = File.OpenRead(fullPath);
        //            if (file != null)
        //            {
        //                stream = new MemoryStream();
        //                file.CopyTo(stream);
        //            }
        //            file.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            _toolbox.MessageBoxException(e);
        //        }
        //    }

        //    return stream;
        //}
        public abstract Models.Document LoadDocument(FileIOInfo info);
        public abstract void SaveDocument(ViewModels.DocumentViewModel document);

        protected string CopyToTemp(string fPath)
        {
            if (string.IsNullOrEmpty(fPath) || !IsFileSupported(fPath)) return string.Empty;

            string tmp = _fileIO.ToTempFileName(fPath);
            string dir = Path.GetDirectoryName(tmp);
            FileStream inputFile;
            FileStream outputFile;


            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                inputFile = File.OpenRead(fPath);
                outputFile = new FileStream(tmp, FileMode.Create);

                inputFile.CopyTo(outputFile);

                inputFile.Close();
                outputFile.Close();
            }
            catch (Exception e)
            {
                tmp = string.Empty;
                _toolbox.MessageBoxException(e);
            }
            if (!string.IsNullOrEmpty(tmp) && !string.IsNullOrWhiteSpace(tmp))
                _tmpFiles.Add(tmp);

            return tmp;
        }
        //public bool TempExists(string fPath)
        //{
        //    return _tmpFiles.Contains(_fileIO.ToTempFileName(fPath));
        //}
        //public void DeleteTempFiles()
        //{
        //    foreach (string file in _tmpFiles)
        //    {
        //        if (File.Exists(file))
        //        {
        //            File.Delete(file);
        //        }
        //    }
        //    _tmpFiles.Clear();
        //}

        protected void SetSupportedExtensions(params string[] extensions)
        {
            // return if already set
            if (this.SupportedExtentions != null) return;

            for (int i = 0; i < extensions.Length; i++)
            {
                extensions[i] = extensions[0].ToUpperInvariant();
                if (extensions[i].StartsWith("."))
                    extensions[i].TrimStart('.');
            }

            SupportedExtentions = extensions.ToArray();
        }
        public bool IsExtensionSupported(string extension)
        {
            extension = extension.Replace(".", "");
            foreach (string ext in SupportedExtentions)
            {
                if(string.Equals(ext.ToUpperInvariant(), extension.ToUpperInvariant()))
                    return true;
            }
            return false;
        }
        public bool IsFileSupported(string fPath)
        {
            return IsExtensionSupported(Path.GetExtension(fPath));
        }
        protected string SafeFilePath(string fPath)
        {
            string dir = Path.GetDirectoryName(fPath);
            string ext = Path.GetExtension(fPath);
            string name = Path.GetFileNameWithoutExtension(fPath);

            if (string.IsNullOrEmpty(dir))
                fPath = _fileIO.SaveDirectoryDefault + fPath;

            if (string.IsNullOrEmpty(ext))
                fPath = fPath + (string.Compare(".", ext) == 0 ? "" : ".") + "pdf";

            return fPath;
        }
    }
}
