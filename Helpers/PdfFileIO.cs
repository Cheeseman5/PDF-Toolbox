using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

using iTextSharp.text.pdf;
using PDFToolbox.Models;
using System.Drawing;

namespace PDFToolbox.Helpers
{
    public class PdfFileIO : BaseFileIOStrategy
    {
        public PdfFileIO(Toolbox toolbox, FileIO fileIO)
            : base(toolbox, fileIO)
        {
            SetSupportedExtensions("PDF");
        }

        public override Models.Document LoadDocument(FileIOInfo info)
        {
            string tmpFile;
            Models.Page page;
            Models.Document doc;
            List<BitmapImage> pageImages = new List<BitmapImage>();
            PdfReader reader;

            //FIXME: handle temp file paths moar better...
            tmpFile = (info.IsTempPath ? info.FullFileName : CopyToTemp(info.FullFileName));
            if (string.IsNullOrEmpty(tmpFile)) return null;


            doc = new Models.Document();

            pageImages = GetPdfPageImages(tmpFile);
            
            reader = new PdfReader(tmpFile);

            for (int i = 0; i < reader.NumberOfPages; i++)
            {
                page = CachePdfPageFromFile(info, reader, i);
                page.image = pageImages[i];
                
                doc.pages.Add(new Document.PageViewModel(page));
            }

            if (doc.pages.Count > 0)
            {
                doc.image = doc.pages[0].Image;
                doc.fName = doc.pages[0].DocName;
            }

            return doc;
        }
        public override void SaveDocument(ViewModels.DocumentViewModel document)
        {
            string srcDocPath;
            string targetFilePath = SafeFilePath(document.DocName);
            Stream stream;
            iTextSharp.text.Image image;
            PdfDictionary pageDict;
            PdfImportedPage importedPage;
            PdfContentByte contentByte;
            PdfCopy targetPdf;
            iTextSharp.text.Document doc;
            PdfReader srcReader;
            PdfCopy.PageStamp pageStamp;

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(targetFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

                stream = new FileStream(targetFilePath, FileMode.Create);

                doc = new iTextSharp.text.Document();
                targetPdf = new PdfCopy(doc, stream);
                doc.Open();

                foreach (ViewModels.PageViewModel vm in document.Pages)
                {
                    srcDocPath = _fileIO.ToTempFileName(vm.DocName);

                    // Copy pageDict from source...
                    if (Path.GetExtension(srcDocPath).ToUpperInvariant() == ".PDF")
                    {
                        srcReader = new iTextSharp.text.pdf.PdfReader(srcDocPath);
                        pageDict = srcReader.GetPageN(vm.Number);
                        importedPage = targetPdf.GetImportedPage(srcReader, vm.Number);
                        pageStamp = targetPdf.CreatePageStamp(importedPage);

                        //add any strings
                        foreach(UIString str in vm.Strings)
                        {
                            ColumnText.ShowTextAligned(pageStamp.GetOverContent(),
                                iTextSharp.text.Element.ALIGN_LEFT,
                                new iTextSharp.text.Phrase(str.String),
                                (float)str.X,
                                (float)(importedPage.Height - str.Y - (str.Height * 0.75)),
                                0);
                        }
                        // apply any added rotation
                        pageDict.Put(PdfName.ROTATE, new PdfNumber((vm.FlatRotation) % 360f));
                        pageStamp.AlterContents();
                        targetPdf.AddPage(importedPage);
                        
                        targetPdf.FreeReader(srcReader);
                        srcReader.Close();
                    }

                    if (vm.ImageStream != null && targetPdf.NewPage())
                    {
                        contentByte = new PdfContentByte(targetPdf);

                        image = iTextSharp.text.Image.GetInstance(vm.ImageStream);

                        image.ScalePercent(72f / image.DpiX * 100);
                        image.SetAbsolutePosition(0, 0);

                        contentByte.AddImage(image);
                        contentByte.ToPdf(targetPdf);

                    }
                }
                targetPdf.Close();
                doc.Close();
                stream.Close();
            }
            catch (IOException e)
            {
                _toolbox.MessageBox(e.Message);
            }
            catch (Exception e)
            {
                _toolbox.MessageBoxException(e);
            }
        }

        private Models.Page CachePdfPageFromFile(FileIOInfo info, PdfReader reader, int pageNum)
        {
            Models.Page page = new Models.Page();
            page.number = ++pageNum;
            page.fName = (info.IsTempPath ? info.FileName : info.FullFileName);
            //FIXME: this is making the pages render with wrong rotation unless rotation is zero
            page.originalRotation = new PdfNumber(reader.GetPageRotation(pageNum));

            return page;
        }

        private BitmapImage GetPdfPageImage(PdfiumViewer.PdfDocument pDoc, int pageNum)
        {
            BitmapImage image = new BitmapImage();
            Bitmap img;
            MemoryStream stream = new MemoryStream();

            img = (Bitmap)pDoc.Render(pageNum, 96.0f, 96.0f, true);
            img.Save(stream, ImageFormat.Bmp);


            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();

            return image;
        }
        private List<BitmapImage> GetPdfPageImages(string fName)
        {
            BitmapImage img;
            List<BitmapImage> images = new List<BitmapImage>();
            PdfiumViewer.PdfDocument pDoc = PdfiumViewer.PdfDocument.Load(fName);

            for (int i = 0; i < pDoc.PageCount; i++)
            {
                img = GetPdfPageImage(pDoc, i);

                if(img!=null)
                {
                    images.Add(img);
                }
                else
                {
                    images.Add(new BitmapImage());
                }
            }

            pDoc.Dispose();
            return images;
        }
    }
}
