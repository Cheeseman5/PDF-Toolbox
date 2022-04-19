using PDFToolbox.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace PDFToolbox.Helpers
{
    public class FileIO
    {
        private Dictionary<string, BaseFileIOStrategy> _fileLoaders = null;
        private List<BaseFileIOExtractor> _fileExtractors = null;
        private List<string> _extractedSrcFiles = null;
        private Toolbox _toolbox;
        private ILogger _logger;

        public string SaveDirectoryDefault
        {
            get { return ".\\PDFToolbox\\"; }
        }
        public string SaveDirectoryTemp
        {
            get { return (string.IsNullOrEmpty(Path.GetTempPath()) ? ".\\temp\\" : Path.GetTempPath()); }
        }
        public string SaveTempPrefix
        {
            get { return "_tmp_"; }
        }

        public FileIO(Toolbox toolbox, ILogger logger)
        {
            _toolbox = toolbox;
            _logger = logger;
        }

        public void RegisterStrategy(BaseIOStrategy strategy)
        {
            if (_fileLoaders == null)
                _fileLoaders = new Dictionary<string, BaseFileIOStrategy>();

            if (_fileExtractors == null)
                _fileExtractors = new List<BaseFileIOExtractor>();

            _logger.Log("FileIO.RegisterStrategy: {0}", strategy.GetType());

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
        public FileIOInfo[] ExtractFileInfo(IDataObject data)
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
        public Models.Document[] ExtractDocument(FileIOInfo[] files)
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
        public Models.Document[] ExtractDocument(IDataObject data)
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
        public Models.Document LoadDocument(FileIOInfo info)
        {
            // Return null if 'fPath' is invalid
            if (info==null || !IsFileValid(info.FullFileName)) return null;

            BaseFileIOStrategy loader;

            loader = GetValidIOStrategy(info.FullFileName);

            // Return null if no strategy available for the file type
            if (loader == null) return null;

            _logger.Log("FileIO.LoadDocument: {0}: {1}", loader.GetType(), info.FullFileName);
            return loader.LoadDocument(info);
        }
        #endregion
        #endregion

        #region Utils
        private BaseFileIOStrategy GetValidIOStrategy(string fPath)
        {
            BaseFileIOStrategy loader;
            string ext = ParseExtension(fPath);

            _logger.Log("FileIO.GetValidIOStrategy(\"{0}\"): Ext: {1}", fPath, ext);

            if(IsFileValid(ext) && _fileLoaders.ContainsKey(ext))
            {
                if (_fileLoaders.TryGetValue(ext, out loader))
                    return loader;
            }

            return null;
        }
        private string ParseExtension(string fPath)
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
        public string ToTempFileName(string fPath)
        {
            return ToTempFileName(fPath, -1);
        }
        public string ToTempFileName(string fPath, int pageNumber)
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
        public bool IsFileValid(string fPath, bool checkFileExistance=false)
        {
            if(string.IsNullOrEmpty(fPath) || string.IsNullOrWhiteSpace(fPath)) return false;

            if(checkFileExistance) return File.Exists(Path.GetFullPath(fPath));

            return true;
        }
        public bool IsExtensionSupported(string file)
        {
            if (!IsFileValid(file)) return false;

            return _fileLoaders.ContainsKey(ParseExtension(file));
        }
        #endregion
    }
}
