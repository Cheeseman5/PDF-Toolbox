using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFToolbox.PDF
{
    public static class PdfConverter
    {
        /*private static Dictionary<string, PdfConvert> _converters = null;

        public static PdfData ToData<T>() where T : PdfConvert
        {
            PdfConvert converter = GetConverter<T>();
            return converter.ConvertToPdfData();
        }

        public static object FromData<T>(PdfData data) where T : PdfConvert
        {
            PdfConvert converter = GetConverter<T>();
            return converter.ConvertFromPdfData(data);
        }

        private static PdfConvert GetConverter<T>() where T : PdfConvert
        {
            if (_converters == null)
                _converters = new Dictionary<string, PdfConvert>();

            PdfConvert converter = null;
            if (_converters.TryGetValue(typeof(T).Name, out converter))
            {
                return converter;
            }
            return null;
        }
        private static bool HaveConverter<T>() where T : PdfConvert
        {
            if (_converters == null) return false;
            return _converters.ContainsKey(typeof(T).Name);
        }*/
    }
}
