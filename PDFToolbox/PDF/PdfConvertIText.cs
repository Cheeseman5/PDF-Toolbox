using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iTextSharp.text.pdf;

namespace PDFToolbox.PDF
{
    public class PdfConvertIText
    {

        public static PdfData ConvertToPdfData(string fileName, int pageNum)
        {
            if ((string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName)) && pageNum <= 0) return null;
            Helpers.D.Log("PdfConvertIText.ConvertToPdfData({0}, {1})", fileName,pageNum);
            PdfData data = new PdfData();
            PdfContentParser parser;// = new PdfContentParser();
            PRTokeniser tokeniser = new PRTokeniser(fileName);
            PdfDictionary dict;
            ArrayList items;
            
            parser = new PdfContentParser(tokeniser);

            dict = parser.ReadDictionary();
            //dict.Contains(PdfName.IMAGE)
            items = parser.Parse(parser.ReadArray().ArrayList);
            Helpers.D.Log("PdfConvertIText.ConvertToPdfData: {0} | {1}", items.Count, string.Join(", ", items.ToArray()));

            return data;
        }

        public static PdfImportedPage ConvertFromPdfData(PdfData data)
        {


            return null;
        }
    }
}
