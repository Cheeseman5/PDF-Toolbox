using System.Collections.ObjectModel;
using Factories;

namespace PDFToolbox.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<DocumentViewModel> _docs = new ObservableCollection<DocumentViewModel>();
        public ObservableCollection<DocumentViewModel> Documents
        {
            get { return _docs; }
        }

        private DocumentViewModel _selectedDoc = null;
        public DocumentViewModel SelectedDocument
        {
            get { return _selectedDoc; }
            set
            {
                _selectedDoc = value;
                OnPropertyChanged("SelectedDocument");
                HaveSelectedDoc = (_selectedDoc != null);
            }
        }

        private bool _haveSelectedDoc = false;
        public bool HaveSelectedDoc
        {
            get { return _haveSelectedDoc; }
            set
            {
                _haveSelectedDoc = value;
                OnPropertyChanged("HaveSelectedDoc");
            }
        }
        #endregion

        public readonly string[] SUPPORTED_FILE_TYPES = { ".PDF" };
        private PageFactory _pageFactory;

        public MainViewModel(PageFactory pageFactory)
        {
            _pageFactory = pageFactory;
        }

        #region Debugging methods
        private DocumentViewModel AddNewDoc(Models.Document doc)
        {
            int nextIndex = (SelectedDocument != null ? Documents.IndexOf(SelectedDocument) : -1) + 1;

            DocumentViewModel docVM = new DocumentViewModel(doc);
            Documents.Insert(nextIndex, docVM);
            //SelectedDocument = docVM;

            return docVM;
        }
        #endregion

        #region Page Modifications
        private void RotatePage(PageViewModel page, float rotate)
        {
            if(page!=null)
                page.Rotation += rotate;
        }
        #endregion

    }
}
