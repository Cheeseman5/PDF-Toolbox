using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;
using System.IO;
using Factories;

namespace PDFToolbox.ViewModels
{
    public class PageViewModel : ViewModelBase
    {
        private Models.Page _page = null;
        private PageFactory _pageFactory;

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

        public PageViewModel(Models.Page page, PageFactory pageFactory)
        {
            SetPage(page);

            _pageFactory = pageFactory;
        }

        public PageViewModel(PageFactory pageFactory)
        {
            _pageFactory = pageFactory;
        }
        public void SetPage(Models.Page page)
        {
            if (page == null)
                throw new ArgumentNullException("pageDict");
            _page = page;
            _page.UIStrings = new ObservableCollection<Models.UIString>();
            _page.UIStrings.CollectionChanged += OnStringsChanged;
        }

        public void Copy(PageViewModel page)
        {
            _pageFactory.CopyPage(page._page);
        }

        //TODO: fix this.  This should probably be pulled out into another class instead of being static.
        public static PageViewModel MakeCopy(PageViewModel page)
        {
            var p = new PageViewModel(new Models.Page(), new PageFactory());

            p.Copy(page);

            return p;
        }

        public BitmapImage Image
        {
            get { return _page.Image; }
            set
            {
                _page.Image = value;
                OnPropertyChanged("Image");
            }
        }

        public string DocName
        {
            get { return _page.FileName; }
            set
            {
                _page.FileName = value;
                OnPropertyChanged("DocName");
            }
        }

        public int Number
        {
            get { return _page.OriginalPageNumber; }
            set
            {
                _page.OriginalPageNumber = value;
                OnPropertyChanged("Number");
            }
        }

        public float Rotation
        {
            get { return _page.Rotation.FloatValue; }
            set
            {
                _page.Rotation = new iTextSharp.text.pdf.PdfNumber(value);
                OnPropertyChanged("Rotation");
            }
        }

        // FIXME: find a better way to handle 2 rotations. Maybe find a way to reduce it down to 1 again...
        public float FlatRotation
        {
            get { return _page.Rotation.FloatValue + _page.OriginalRotation.FloatValue; }
        }

        public Stream ImageStream
        {
            get { return _page.ImageStream; }
            set
            {
                _page.ImageStream = value;
                OnPropertyChanged("ImageStream");
            }
        }

        public ObservableCollection<Models.UIString> Strings
        {
            get { return _page.UIStrings; }
            private set
            {
                _page.UIStrings = value;
                OnPropertyChanged("Strings");
            }
        }
        
        public int ID
        {
            get { return _page.ID; }
        }

        public void OnStringsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Notify that Count may have been changed
            OnPropertyChanged("StringsCount");


        }
    }
}
