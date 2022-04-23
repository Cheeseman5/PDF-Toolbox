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
        private List<string> _tmpFiles;
        private string _defaultSaveDirectory;
        public string[] SupportedExtentions { get; set; }
        public PdfFileIO(Toolbox toolbox, FileIO fileIO, PageFactory pageFactory, string defaultSaveDirectory)
        {
            _pageFactory = pageFactory;
            _toolbox = toolbox;
            _fileIO = fileIO;
            _tmpFiles = new List<string>();
            _defaultSaveDirectory = defaultSaveDirectory;
            SupportedExtentions = new string[1];

            SetSupportedExtensions("PDF");
        }

        public Document LoadDocument(Models.FileIOInfo info)
        {
            string tmpFile;
            Page page;
            Document document;
            List<BitmapImage> pageImages = new List<BitmapImage>();
            PdfReader reader;

            //FIXME: handle temp file paths moar better...
            tmpFile = (info.IsTempPath ? info.FullFileName : CopyToTemp(info.FullFileName));
            if (string.IsNullOrEmpty(tmpFile)) return null;


            document = _pageFactory.CreateDocument();

            pageImages = GetPdfPageImages(tmpFile);
            
            reader = new PdfReader(tmpFile);

            for (int i = 0; i < reader.NumberOfPages; i++)
            {
                page = CachePdfPageFromFile(info, reader, i);
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
        public string CopyToTemp(string fPath)
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
            string dir = Path.GetDirectoryName(fPath);
            string ext = Path.GetExtension(fPath);
            string name = Path.GetFileNameWithoutExtension(fPath);

            if (string.IsNullOrEmpty(dir))
                fPath = _defaultSaveDirectory + fPath;

            if (string.IsNullOrEmpty(ext))
                fPath = fPath + (string.Compare(".", ext) == 0 ? "" : ".") + "pdf";

            return fPath;
        }

        private Page CachePdfPageFromFile(FileIOInfo info, PdfReader reader, int pageNum)
        {
            Page page = new Page();
            page.OriginalPageNumber = ++pageNum;
            page.FileName = (info.IsTempPath ? info.FileName : info.FullFileName);
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
