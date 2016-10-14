using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;
using System.IO;

using PDFToolbox.Common.ViewModels;

namespace PDFToolbox.ViewModels
{
    public class PageViewModel : ViewModelBase
    {
        private Models.Page _page = null;

        private double _scale;
        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnPropertyChanged("Scale");
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public PageViewModel(Models.Page page)
        {
            SetPage(page);
        }

        public PageViewModel()
        {
        }
        public void SetPage(Models.Page page)
        {
            if (page == null)
                throw new ArgumentNullException("pageDict");
            _page = page;
            _page.uiStrings = new ObservableCollection<Common.UIString>();
            _page.uiStrings.CollectionChanged += OnStringsChanged;
        }

        public void Copy(PageViewModel page)
        {
            this._page.Copy(page._page);
        }
        public static PageViewModel MakeCopy(PageViewModel page)
        {
            PageViewModel p = new PageViewModel(new Models.Page());

            p.Copy(page);

            return p;
        }

        public BitmapImage Image
        {
            get { return _page.image; }
            set
            {
                _page.image = value;
                OnPropertyChanged("Image");
            }
        }

        public string DocName
        {
            get { return _page.fName; }
            set
            {
                _page.fName = value;
                OnPropertyChanged("DocName");
            }
        }

        public int Number
        {
            get { return _page.number; }
            set
            {
                _page.number = value;
                OnPropertyChanged("Number");
            }
        }

        public float Rotation
        {
            get { return _page.rotation.FloatValue; }
            set
            {
                _page.SetRotation(value);
                OnPropertyChanged("Rotation");
            }
        }

        // FIXME: find a better way to handle 2 rotations. Maybe find a way to reduce it down to 1 again...
        public float FlatRotation
        {
            get { return _page.rotation.FloatValue + _page.originalRotation.FloatValue; }
            /*set
            {
                _page.SetRotation(value);
                OnPropertyChanged("Rotation");
            }*/
        }

        // Delete?
        /*public bool IsImagePreview
        {
            get { return _page.isImagePreview; }
            set
            {
                _page.isImagePreview = value;
                OnPropertyChanged("IsImagePreview");
            }
        }*/

        public Stream ImageStream
        {
            get { return _page.imageStream; }
            set
            {
                _page.imageStream = value;
                OnPropertyChanged("ImageStream");
            }
        }

        public ObservableCollection<Common.UIString> Strings
        {
            get { return _page.uiStrings; }
            private set
            {
                _page.uiStrings = value;
                OnPropertyChanged("Strings");
            }
        }
        
        public int ID
        {
            get { return _page.id; }
        }

        public void OnStringsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Notify that Count may have been changed
            OnPropertyChanged("StringsCount");


        }
    }
}
