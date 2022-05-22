using System.IO;

namespace PDFToolbox.Models
{
    public class FileIOInfo
    {
        public Stream Stream;

        private string _fullPath = "";

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

        public bool IsTempPath { get; set; }

        /// <summary>
        /// Gets the fully usable file path name. <para>Sets the fully usable file path name.</para>
        /// </summary>
        public string FullFileName;
    }
}
