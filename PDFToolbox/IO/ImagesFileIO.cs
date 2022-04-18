using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace PDFToolbox.IO
{
    public class ImagesFileIO : BaseFileIOStrategy
    {
        public ImagesFileIO()
            : base()
        {
            SetSupportedExtensions("TIF", "TIFF");
        }

        public override Models.Document LoadDocument(FileIOInfo info)
        {
            if (info == null) return null;

            TiffBitmapDecoder tiff = new TiffBitmapDecoder(info.Stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            Models.Document doc = new Models.Document();
            Models.Page page;

            for(int i = 0; i<tiff.Frames.Count; i++)
            //foreach (BitmapFrame frame in tiff.Frames)
            {
                page = new Models.Page();

                page.FileName = info.FullFileName;
                page.Image = TiffFrameToBitmapImage(tiff.Frames[i]);
                page.OriginalPageNumber = i+1;
                page.ImageStream = (MemoryStream)page.Image.StreamSource;

                doc.Pages.Add(page);
            }


            return doc;
        }

        public override void SaveDocument(ViewModels.DocumentViewModel document)
        {
            throw new NotImplementedException();
        }

        private BitmapImage TiffFrameToBitmapImage(BitmapFrame frame)
        {
            int stride = (int)frame.PixelWidth * ((frame.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[(int)frame.PixelHeight * stride];
            BitmapImage img;;
            
            frame.CopyPixels(pixels, stride, 0);

            img = new BitmapImage();
            //img.CopyPixels(pixels, stride, 0);
            

            return img;
        }
    }
}
