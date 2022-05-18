using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

using iTextSharp.text.pdf;
using PDFToolbox.Models;
using PDFToolbox.Interfaces;
using System.Drawing;
using Factories;
using PDFToolbox.Interfaces.Helpers;
using System.Linq;

namespace PDFToolbox.Helpers
{
    public class PdfFileIO : IFileIOStrategy
    {
        private PageFactory _pageFactory;
        private Toolbox _toolbox;
        private IFileIO _fileIO;
        private string _defaultSaveDirectory;
        public string[] SupportedExtentions { get; set; }
        public PdfFileIO(Toolbox toolbox, IFileIO fileIO, PageFactory pageFactory, string defaultSaveDirectory)
        {
            _pageFactory = pageFactory;
            _toolbox = toolbox;
            _fileIO = fileIO;
            _defaultSaveDirectory = defaultSaveDirectory;
            SupportedExtentions = new string[1];

            SetSupportedExtensions("PDF");
        }

        public Document LoadDocument(Models.FileIOInfo info)
        {
            //FIXME: handle temp file paths moar better...
            string tmpFile = CopyToTemp(info.FullFileName));
            if (info.IsTempPath)
            {
                tmpFile = info.FullFileName;
            }
            if (string.IsNullOrEmpty(tmpFile)) return null;

            Document document = ConstructDocument(tmpFile, info);

            return document;
        }
        private Document ConstructDocument(string filePath, Models.FileIOInfo info)
        {
            Document document = _pageFactory.CreateDocument();

            List<BitmapImage> pageImages = GetPdfPageImages(filePath);
            var reader = new PdfReader(filePath);
            for (int i = 0; i < reader.NumberOfPages; i++)
            {
                Page page = CachePdfPageFromFile(info, reader, i);
                page.Image = pageImages[i];

                document.Pages.Add(_pageFactory.CopyPage(page));
            }

            return document;
        }
        public void SaveDocument(ViewModels.DocumentViewModel document)
        {
            string srcDocPath;
            string targetFilePath = SafeFilePath(document.DocName);
            Stream stream;
            PdfCopy targetPdf;
            iTextSharp.text.Document doc;

            try
            {
                _fileIO.CreateDirectory(targetFilePath);

                stream = new FileStream(targetFilePath, FileMode.Create);
                doc = new iTextSharp.text.Document();
                targetPdf = new PdfCopy(doc, stream);
                doc.Open();

                foreach (ViewModels.PageViewModel vm in document.Pages)
                {
                    srcDocPath = _fileIO.ToTempFileName(vm.DocName);

                    AddPdfPage(vm, srcDocPath, targetPdf);
                    AddImageToPage(vm, targetPdf);
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
        private void AddPdfPage(ViewModels.PageViewModel page, string srcDocPath, PdfCopy targetPdf)
        {
            if (Path.GetExtension(srcDocPath).ToUpperInvariant() != ".PDF")
                return;
            
            PdfReader srcReader = new iTextSharp.text.pdf.PdfReader(srcDocPath);
            PdfImportedPage preparedPage;

            preparedPage = PreparePdfPage(page, srcReader, targetPdf);
            
            targetPdf.AddPage(preparedPage);

            targetPdf.FreeReader(srcReader);
            srcReader.Close();
        }

        private PdfImportedPage PreparePdfPage(ViewModels.PageViewModel page, PdfReader srcReader, PdfCopy targetPdf)
        {
            PdfDictionary pageDict = srcReader.GetPageN(page.Number);
            PdfImportedPage importedPage = targetPdf.GetImportedPage(srcReader, page.Number);
            PdfCopy.PageStamp pageStamp = targetPdf.CreatePageStamp(importedPage);

            foreach (UIString str in page.Strings)
            {
                ColumnText.ShowTextAligned(pageStamp.GetOverContent(),
                    iTextSharp.text.Element.ALIGN_LEFT,
                    new iTextSharp.text.Phrase(str.String),
                    (float)str.X,
                    (float)(importedPage.Height - str.Y - (str.Height * 0.75)),
                    0);
            }
            // apply any added rotation
            pageDict.Put(PdfName.ROTATE, new PdfNumber((page.FlatRotation) % 360f));
            pageStamp.AlterContents();

            return importedPage;
        }

        private void AddImageToPage(ViewModels.PageViewModel page, PdfCopy targetPdf)
        {
            if (page.ImageStream != null && targetPdf.NewPage())
            {
                var contentByte = new PdfContentByte(targetPdf);

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(page.ImageStream);

                image.ScalePercent(72f / image.DpiX * 100);
                image.SetAbsolutePosition(0, 0);

                contentByte.AddImage(image);
                contentByte.ToPdf(targetPdf);

            }
        }
        public string CopyToTemp(string fPath)
        {
            if (string.IsNullOrEmpty(fPath) || !IsFileSupported(fPath)) return string.Empty;

            string tmp = _fileIO.ToTempFileName(fPath);

            _fileIO.CopyToTemp(fPath);

            return tmp;
        }
        public void SetSupportedExtensions(params string[] extensions)
        {
            if (SupportedExtentions != null) return;

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
                if (string.Equals(ext.ToUpperInvariant(), extension.ToUpperInvariant()))
                    return true;
            }
            return false;
        }
        public bool IsFileSupported(string fPath)
        {
            return IsExtensionSupported(Path.GetExtension(fPath));
        }
        public string SafeFilePath(string fPath)
        {
            string safeFilePath = _fileIO.MakeFilePathSafe(fPath, _defaultSaveDirectory);
            return safeFilePath;
        }

        private Page CachePdfPageFromFile(Models.FileIOInfo info, PdfReader reader, int pageNum)
        {
            Page page = new Page();
            page.OriginalPageNumber = ++pageNum;
            page.FileName = info.FullFileName;

            if (info.IsTempPath)
                page.FileName = info.FileName;

            //FIXME: this is making the pages render with wrong rotation unless rotation is zero
            page.OriginalRotation = new PdfNumber(reader.GetPageRotation(pageNum));

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

                if(img==null)
                {
                    img = new BitmapImage();
                }
                
                images.Add(img);
            }

            pDoc.Dispose();
            return images;
        }
    }
}
