using PDFToolbox.Interfaces;
using System.Collections.Specialized;
using System.Configuration;

namespace Helpers
{
    public class DefaultConfig : IConfig
    {
        private NameValueCollection _appSettings;
        public string DefaultSaveDirectory => _appSettings["DefaultSaveDirectory"];

        public string DefaultTempSaveDirectory => _appSettings["DefaultTempSaveDirectory"];

        public string TempSavePrefix => _appSettings["TempSavePrefix"];

        public string AppVersion => _appSettings["AppVersion"];
        public string AppName => _appSettings["AppName"];
        public bool SilenceMessagebox => bool.Parse(_appSettings["SilenceMessagebox"]);

        public DefaultConfig()
        {
            _appSettings = ConfigurationManager.AppSettings;
        }
    }
}
