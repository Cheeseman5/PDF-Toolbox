﻿using System.IO;

namespace PDFToolbox.Helpers
{
    public class FileIOInfo
    {
        public Stream Stream { get; set; }
        
        private string _fullPath = "";
        private bool _tempPath = false;

        /// <summary>
        /// Gets the file name and extension of FileIOInfo.FullFileName. <para>Sets the file name but not the extension of FileIOInfo.FullFileName.</para>
        /// </summary>
        public string FileName 
        {
            get { return Path.GetFileName(_fullPath); }
            set { FullFileName = Directory + value + Extension; }
        }

        /// <summary>
        /// Gets the file extension of FileIOInfo.FullFileName. <para>Sets the file extension of FileIOInfo.FullFileName.</para>
        /// </summary>
        public string Extension
        {
            get { return Path.GetExtension(_fullPath); }
            set { FullFileName = Directory + FileName + value; }
        }
        /// <summary>
        /// Gets the full directory of FileIOInfo.FullFileName. <para>Sets the full directory of FileIOInfo.FullFileName.</para>
        /// </summary>
        public string Directory
        {
            get { return Path.GetDirectoryName(_fullPath); }
            set { FullFileName = value + Path.GetFileName(_fullPath); }
        }
        /// <summary>
        /// Gets the fully usable file path name. <para>Sets the fully usable file path name.</para>
        /// </summary>
        public string FullFileName
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }
        /// <summary>
        /// True - FullFileName names a file that is in the temporary directory. <para>False - FullFileName names a file that is not in the temporary directory.</para>
        /// </summary>
        public bool IsTempPath
        {
            get { return _tempPath; }
            set { _tempPath = value; }
        }
    }
}
