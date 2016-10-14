using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace PDFToolbox.IO
{
    public static class FileIO
    {
        private static Dictionary<string, BaseFileIOStrategy> _fileLoaders = null;
        private static List<BaseFileIOExtractor> _fileExtractors = null;
        private static List<string> _extractedSrcFiles = null;

        public static string SaveDirectoryDefault
        {
            get
            {
                return ".\\PDFToolbox\\";
            }
        }
        public static string SaveDirectoryTemp
        {
            get
            {
                return (string.IsNullOrEmpty(Path.GetTempPath()) ? ".\\temp\\" : Path.GetTempPath());
            }
        }
        public static string SaveTempPrefix
        {
            get { return "_tmp_"; }
        }

        public static void RegisterStrategy(BaseIOStrategy strategy)
        {
            if (_fileLoaders == null)
                _fileLoaders = new Dictionary<string, BaseFileIOStrategy>();

            if (_fileExtractors == null)
                _fileExtractors = new List<BaseFileIOExtractor>();

            Helpers.D.Log("FileIO.RegisterStrategy: {0}", strategy.GetType());

            // File IO
            if (strategy is BaseFileIOStrategy)
            {
                string[] exts = ((BaseFileIOStrategy)strategy).SupportedExtentions;

                foreach (string ext in exts)
                {
                    if (!_fileLoaders.ContainsKey(ext))
                        _fileLoaders.Add(ext.ToUpperInvariant(), strategy as BaseFileIOStrategy);
                }
            }

            // File extractor
            if (strategy is BaseFileIOExtractor)
            {
                _fileExtractors.Add(strategy as BaseFileIOExtractor);
            }
        }

        #region Interface
        #region Extraction
        public static FileIOInfo[] ExtractFileInfo(IDataObject data)
        {
            if (data == null) return null;

            //List<FileIOInfo> files = new List<FileIOInfo>();
            FileIOInfo[] files;

            foreach (BaseFileIOExtractor extractor in _fileExtractors)
            {
                files = extractor.GetFileStreams(data);

                if (files != null && files.Length > 0)
                    return files;
            }
            return null;
        }
        public static Models.Document[] ExtractDocument(FileIOInfo[] files)
        {
            if (files == null || files.Length <= 0) return null;

            List<Models.Document> docs = new List<Models.Document>();
            Models.Document doc;

            foreach (FileIOInfo info in files)
            {
                doc = LoadDocument(info);

                if (doc != null)
                    docs.Add(doc);
            }
            // load documents

            return docs.ToArray();
        }
        public static Models.Document[] ExtractDocument(IDataObject data)
        {
            if (data == null) return null;

            List<Models.Document> docs;

            FileIOInfo[] fileInfos = ExtractFileInfo(data);
            List<FileIOInfo> files;

            if (fileInfos == null || fileInfos.Length <= 0) return null;

            files = new List<FileIOInfo>(fileInfos);

            if (files.Count > 0)
            {
                docs = new List<Models.Document>();
                docs.AddRange(ExtractDocument(files.ToArray()));
                return docs.ToArray();
            }
            return null;
        }

        #endregion
        #region IO
        public static MemoryStream Load(string fPath)
        {
            if (IsFileValid(fPath)) return null;

            BaseFileIOStrategy loader = GetValidIOStrategy(fPath);

            if (loader == null) return null;
            Helpers.D.Log("FileIO.Load: {0}", fPath);
            return loader.Load(fPath);
        }
        public static Models.Document LoadDocument(FileIOInfo info)
        {
            // Return null if 'fPath' is invalid
            if (info==null || !IsFileValid(info.FullFileName)) return null;

            BaseFileIOStrategy loader;

            loader = GetValidIOStrategy(info.FullFileName);

            // Return null if no strategy available for the file type
            if (loader == null) return null;

            Helpers.D.Log("FileIO.LoadDocument: {0}: {1}", loader.GetType(), info.FullFileName);
            return loader.LoadDocument(info);
        }
        public static void SaveDocument(ViewModels.DocumentViewModel document)
        {
            if (document == null) return;

            // Default file type is 'PDF' if none provided in docName (ext was omited)
            string ext = "";
            if(string.IsNullOrEmpty(ParseExtension(document.DocName))) ext = ".PDF";

            BaseFileIOStrategy strategy = GetValidIOStrategy(document.DocName + ext);

            if (strategy == null) return;

            strategy.SaveDocument(document);
        }
        #endregion
        #endregion

        #region Utils
        private static BaseFileIOStrategy GetValidIOStrategy(string fPath)
        {
            BaseFileIOStrategy loader;
            string ext = ParseExtension(fPath);

            Helpers.D.Log("FileIO.GetValidIOStrategy(\"{0}\"): Ext: {1}", fPath, ext);

            if(IsFileValid(ext) && _fileLoaders.ContainsKey(ext))
            {
                if (_fileLoaders.TryGetValue(ext, out loader))
                    return loader;
            }

            return null;
        }
        private static string ParseExtension(string fPath)
        {
            string ext = string.Empty;

            if (IsFileValid(fPath))
            {
                 ext = Path.GetExtension(fPath);

                 if (!string.IsNullOrEmpty(ext) && !string.IsNullOrWhiteSpace(ext))
                     ext = ext.ToUpperInvariant().Replace(".", "");
            }
            return ext;
        }
        private static bool SaveTemp(FileIOInfo info)
        {
            if (info == null || info.Stream==null || info.Stream.Length==0) return false;

            if (_extractedSrcFiles == null)
                _extractedSrcFiles = new List<string>();

            try
            {
                FileStream file;
                string tmpPath = ToTempFileName(info.FileName);

                file = new FileStream(tmpPath, FileMode.Create);
                info.Stream.CopyTo(file, (int)info.Stream.Length);

                _extractedSrcFiles.Add(tmpPath);
            }
            catch (Exception e)
            {
                Toolbox.MessageBoxException(e);
                return false;
            }
            return true;
        }
        public static bool TempFileExists(string fPath)
        {
            if(!IsFileValid(fPath)) return false;

            BaseFileIOStrategy loader = GetValidIOStrategy(fPath);

            if (loader == null) return false;

            return loader.TempExists(fPath);
        }
        public static string ToTempFileName(string fPath)
        {
            return ToTempFileName(fPath, -1);
        }
        public static string ToTempFileName(string fPath, int pageNumber)
        {
            string tmpPath = SaveDirectoryTemp;

            // Construct temp path
            string path = tmpPath + SaveTempPrefix +        // path
                Path.GetFileNameWithoutExtension(fPath) +   // file name
                (pageNumber >= 0 ? "-" + pageNumber : "") + // pageDict number if > 0
                Path.GetExtension(fPath);                   // extention

            // Using FileInfo to handle extra path construction (i.e. ".\", "%temp%", etc.)
            FileInfo info = new FileInfo(path);
            return info.FullName;
        }
        public static bool IsFileValid(string fPath, bool checkFileExistance=false)
        {
            if(string.IsNullOrEmpty(fPath) || string.IsNullOrWhiteSpace(fPath)) return false;

            if(checkFileExistance) return File.Exists(Path.GetFullPath(fPath));

            return true;
        }
        public static bool IsExtensionSupported(string file)
        {
            if (!IsFileValid(file)) return false;

            return _fileLoaders.ContainsKey(ParseExtension(file));
        }
        public static bool IsExtensionSupported(string[] files)
        {
            if (files == null) return false;

            foreach (string file in files)
            {
                if (!IsExtensionSupported(file))
                    return false;
            }
            return true;
        }


        public static void Cleanup()
        {
            foreach (KeyValuePair<string, BaseFileIOStrategy> entry in _fileLoaders)
            {
                if (entry.Value == null) continue;

                entry.Value.DeleteTempFiles();
            }
            // Delete extracted files
            if (_extractedSrcFiles != null && _extractedSrcFiles.Count > 0)
            {
                foreach (string file in _extractedSrcFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Toolbox.MessageBoxException(e);
                    }
                }
                _extractedSrcFiles.Clear();
            }
        }
        #endregion
    }
}
