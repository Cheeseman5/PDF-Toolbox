using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PDFToolbox.ViewModels
{
    public class DocumentViewModel : ViewModelBase
    {
        private Models.Document _doc = null;
        public Models.Document Document
        { 
            get 
            { 
                return _doc; 
            } 
        }
        public ObservableCollection<ViewModels.PageViewModel> Pages { get; private set; }

        public DocumentViewModel(Models.Document document)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            _doc = document;

            Pages = null;// new ObservableCollection<PageViewModel>(_doc.pages);
            Pages.CollectionChanged += OnPagesChanged;
        }

        #region Properties
        public BitmapImage Image
        {
            get { return _doc.Image; }
            set
            {
                _doc.Image = value;
                OnPropertyChanged("Image");
            }
        }

        public string DocName
        {
            get { return _doc.FileName; }
            set
            {
                _doc.FileName = value;
                OnPropertyChanged("DocName");
            }
        }

        public int PageCount
        {
            get { return Pages.Count; }
        }

        public float Rotation
        {
            get { return (Pages != null && Pages.Count > 0 ? Pages[0].Rotation : 0f); }
        }
        #endregion

        #region Utils
        private void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Notify that Count may have been changed
            OnPropertyChanged("PageCount");

            if (0 < Pages.Count)
                Image = Pages[0].Image;
            else
                Image = null;
        }

        public int GetPageIndex(PageViewModel page)
        {
            return Pages.IndexOf(Pages.Where(p => p.ID == page.ID).FirstOrDefault());
        }
        #endregion
    }
}
