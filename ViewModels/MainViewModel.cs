using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Factories;

namespace PDFToolbox.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public ObservableCollection<PageViewModel> Pages
        {
            get
            {
                if (HaveSelectedDoc)
                    return SelectedDocument.Pages;
                return null;
            }
        }
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

        private bool _haveSelectedPage = false;
        public bool HaveSelectedPage
        {
            get { return _haveSelectedPage; }
            set
            {
                _haveSelectedPage = value;
                OnPropertyChanged("HaveSelectedPage");
            }
        }

        private PageViewModel _selectedPage = null;
        public PageViewModel SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                OnPropertyChanged("SelectedPage");
                HaveSelectedPage = (_selectedPage != null);
            }
        }
        #endregion

        public readonly string[] SUPPORTED_FILE_TYPES = { ".PDF" };
        private PageFactory _pageFactory;
        
        // Needed for data-binding in XML: MainWindow.xaml
        public MainViewModel()
        {
            _pageFactory = new PageFactory();
        }
        public MainViewModel(PageFactory pageFactory)
        {
            _pageFactory = pageFactory;
        }
        public void SplitDocument(DocumentViewModel docVM, int splitInterval)
        {
            if (docVM == null || splitInterval <= 0)
            {
                return;
            }

            DocumentViewModel newDocVM = docVM;

            while (docVM.PageCount > splitInterval)
            {
                // current doc VM reached its goal page-count - start the next one...
                if (newDocVM.PageCount >= splitInterval)
                {
                    Models.Document newDoc = _pageFactory.CreateDocument();

                    newDoc.FileName = docVM.DocName;
                    newDoc.Image = docVM.Pages[0].Image;

                    newDocVM = new DocumentViewModel(newDoc);
                    Documents.Add(newDocVM);
                }
                else
                {
                    newDocVM.Pages.Add(docVM.Pages[splitInterval]);
                    docVM.Pages.RemoveAt(splitInterval);
                }
            }
        }

        public void CachePages(Models.Document[] documents)
        {
            // Exit now if nothing loaded
            if (documents == null || documents.Length == 0)
                return;

            foreach (Models.Document doc in documents)
            {
                // If no document is selected -> Select & add first document to list & append the rest to it
                if (SelectedDocument == null)
                {
                    Documents.Add(new DocumentViewModel(doc));
                    SelectedDocument = Documents[Documents.Count - 1];
                }
            }
        }
        public void CopyDocumentTo(DocumentViewModel source, DocumentViewModel target)
        {
            if (source == null || target == null)
            {
                return;
            }

            foreach (PageViewModel page in source.Pages)
            {
                target.Pages.Add(PageViewModel.MakeCopy(page));
            }
        }
        public void MovePageToDoc(PageViewModel page, DocumentViewModel oldDoc, DocumentViewModel newDoc)
        {
            if (null == oldDoc)
            {
                throw new NullReferenceException("oldDoc");
            }
            if (oldDoc.Pages == null || !oldDoc.Pages.Contains(page))
            {
                throw new KeyNotFoundException(page + " not contained within " + oldDoc.Pages);
            }
            if (null == newDoc)
            {
                throw new NullReferenceException("newDoc");
            }

            oldDoc.Pages.Remove(page);
            newDoc.Pages.Add(page);
        }

        public void CacheDocuments(Models.Document[] documents)
        {
            if (documents == null || documents.Length == 0)
            {
                return;
            }

            DocumentViewModel docVM;
            foreach (Models.Document doc in documents)
            {
                docVM = new DocumentViewModel(doc);
                Documents.Add(docVM);
                SelectedDocument = docVM;
            }
        }
        public bool IsValidDropItem(IDataObject data)
        {
            data.GetData(DataFormats.FileDrop);
            return (
                (
                    data.GetDataPresent(typeof(DocumentViewModel)) ||
                    data.GetDataPresent(typeof(PageViewModel))
                ));
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

        public void MovePage(int oldIndex, int newIndex)
        {
            SelectedDocument.Pages.Move(oldIndex, newIndex);
        }

        public void MoveDocument(int oldIndex, int newIndex)
        {
            Documents.Move(oldIndex, newIndex);
        }
        private void RotatePage(PageViewModel page, float rotate)
        {
            if(page!=null)
                page.Rotation += rotate;
        }
        #endregion

    }
}
