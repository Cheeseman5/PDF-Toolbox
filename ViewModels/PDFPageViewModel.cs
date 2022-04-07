using System.Windows.Media.Imaging;

namespace PDFToolbox.ViewModels
{
    class PDFPageViewModel : ViewModelBase
    {
        private double _scale = 1.0;
        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnPropertyChanged("Scale");
            }
        }

        private BitmapImage _image = null;
        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }



    }
}
