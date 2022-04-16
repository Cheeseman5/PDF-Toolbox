using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Factories;

namespace PDFToolbox.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Event declarations
        private ICommand _addDoc = null;
        public ICommand AddDoc
        {
            get { return _addDoc; }
        }

        private ICommand _saveDoc = null;
        public ICommand SaveDoc
        {
            get { return _saveDoc; }
        }

        private ICommand _saveAllDocs = null;
        public ICommand SaveAllDocs
        {
            get { return _saveAllDocs; }
        }
        private ICommand _rotPageCW90 = null;
        public ICommand RotPageCW90
        {
            get { return _rotPageCW90; }
        }
        private ICommand _rotPageCCW90 = null;
        public ICommand RotPageCCW90
        {
            get { return _rotPageCCW90; }
        }
        private ICommand _splitDoc = null;
        public ICommand SplitDoc
        {
            get { return _splitDoc; }
        }


        private ICommand _clearDocs = null;
        public ICommand ClearDocs
        {
            get { return _clearDocs; }
        }

        private ICommand _removePage = null;
        public ICommand RemovePage
        {
            get { return _removePage; }
        }

        private ICommand _removeDoc = null;
        public ICommand RemoveDoc
        {
            get { return _removeDoc; }
        }

        private ICommand _clearCanvas = null;
        public ICommand ClearCanvas
        {
            get { return _clearCanvas; }
        }

        private ICommand _leftMouseUpCanvas = null;
        public ICommand LeftMouseUpCanvas
        {
            get { return _leftMouseUpCanvas; }
        }

        #endregion
        #region Key Bindings
        private ICommand _keyDeletePressed = null;
        public ICommand KeyDeletePressed
        {
            get { return _keyDeletePressed; }
        }
        #endregion

        #region Properties
        private ObservableCollection<DocumentViewModel> _docs = new ObservableCollection<DocumentViewModel>();
        public ObservableCollection<DocumentViewModel> Documents
        {
            get { return _docs; }
        }

        public ObservableCollection<PageViewModel> Pages
        {
            get
            {
                if (HaveSelectedDoc)
                    return SelectedDocument.Pages;
                return null;
            }
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

        private ObservableCollection<PageViewModel> _selectedPages = null;
        public ObservableCollection<PageViewModel> SelectedPages
        {
            get { return _selectedPages; }
            set
            {
                _selectedPages = value;
                OnPropertyChanged("SelectedPages");
                HaveSelectedPage = (_selectedPages != null && _selectedPages.Count > 0);
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

        private bool _dragOverPageViewValid = false;
        public bool DragOverPageViewValid
        {
            get { return _dragOverPageViewValid; }
            set
            {
                _dragOverPageViewValid = value;
                OnPropertyChanged("DragOverPageViewValid");
            }
        }

        private bool _dragOverPageViewVisible = false;
        public Visibility DragOverPageViewVisible
        {
            get { return (_dragOverPageViewVisible? Visibility.Visible : Visibility.Hidden); }
            set
            {
                _dragOverPageViewVisible = (Visibility.Visible==value);
                OnPropertyChanged("DragOverPageViewVisible");
            }
        }

        private PageViewModel _editPage = null;
        public PageViewModel EditPage
        {
            get { return _editPage; }
            set
            {
                _editPage = value;
                OnPropertyChanged("EditPage");
                IsEditingPage = (_editPage != null);
            }
        }

        private bool _isEditingPage = false;
        public bool IsEditingPage
        {
            get { return _isEditingPage; }
            set
            {
                _isEditingPage = value;
                OnPropertyChanged("IsEditingPage");
            }
        }

        private string _textToAddToCanvas = "";
        public string TextToAddToCanvas
        {
            get { return _textToAddToCanvas; }
            set
            {
                _textToAddToCanvas = value;
                OnPropertyChanged("TextToAddToCanvas");
            }
        }

        private bool _toggleButtonAddText = false;
        public bool ToggleButtonAddText
        {
            get { return _toggleButtonAddText; }
            set
            {
                _toggleButtonAddText = value;
                OnPropertyChanged("ToggleButtonAddText");
            }
        }

        #endregion

        public readonly string[] SUPPORTED_FILE_TYPES = { ".PDF" };
        private PageFactory _pageFactory;

        public MainViewModel(PageFactory pageFactory)
        {
            _pageFactory = pageFactory;
        }

        #region Event implimentations
        private void OnAddDoc(object param)
        {
            AddNewDoc(_pageFactory.CreateDocument($"Doc{_docs.Count}"));
        }

        private void OnSaveDoc(object param)
        {
            if (!HaveSelectedDoc)
                return;

            //IO.FileIO.SaveDocument(SelectedDocument);
        }

        private void OnSaveAllDocs(object param)
        {
            if (Documents.Count==0)
                return;

            for (int i = 0; i < Documents.Count; i++)
            {
                //IO.FileIO.SaveDocument(Documents[i]);
            }
        }

        private void OnRotatePageCW90(object param)
        {
            RotatePage(SelectedPage, 90f);
        }
        private void OnRotatePageCCW90(object param)
        {
            RotatePage(SelectedPage, -90f);
        }

        private void OnSplitDoc(object param) { }


        private void OnClearDocs(object param)
        {
            Documents.Clear();
            SelectedDocument = null;

            //IO.FileIO.Cleanup();
        }

        private void OnRemovePage(object param)
        {
            if (!HaveSelectedPage)
                return;

            int idx = SelectedDocument.GetPageIndex(SelectedPage);
            SelectedDocument.Pages.RemoveAt(idx);

            if (SelectedDocument.PageCount > 0)
            {
                //Helpers.D.Log("MainViewModel::OnRemovePage: pageDict.count:{0}", SelectedDocument.PageCount);
                SelectedPage = SelectedDocument.Pages.ElementAt(Math.Min(Math.Max(0, SelectedDocument.Pages.Count - 1), idx));
            }
            else
                SelectedPage = null;
        }

        private void OnRemoveDoc(object param)
        {
            if (!HaveSelectedDoc)
                return;

            Documents.Remove(SelectedDocument);
        }

        private void OnKeyDeletePressed(object param)
        {
            if (!HaveSelectedPage)
                _removeDoc.Execute(param);
            else
                _removePage.Execute(param);
        }

        private void OnLeftMouseUpCanvas(object param) { }

        private void OnClearCanvas(object param)
        {
            SelectedPage.Strings.Clear();
        }
        #endregion

        #region Debugging methods
        private DocumentViewModel AddNewDoc(Models.Document doc)
        {
            int nextIndex = (SelectedDocument != null ? Documents.IndexOf(SelectedDocument) : -1) + 1;

            DocumentViewModel docVM = new DocumentViewModel(doc);
            Documents.Insert(nextIndex, docVM);
            //SelectedDocument = docVM;

            return docVM;
        }

        private PageViewModel AddNewPage(Models.Page page)
        {
            int nextIndex = (SelectedPage != null ? Pages.IndexOf(SelectedPage) : -1) + 1;

            PageViewModel pageVM = new PageViewModel(page, _pageFactory);
            SelectedDocument.Pages.Insert(nextIndex, pageVM);
            SelectedPage = pageVM;

            return pageVM;
        }
        #endregion

        #region Page Modifications
        private void RotatePage(PageViewModel page, float rotate)
        {
            if(page!=null)
                page.Rotation += rotate;
        }
        #endregion

        #region Adding new documents/pages
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
                    SelectedDocument = Documents[Documents.Count-1];
                }
            }
        }

        public void CacheDocuments(Models.Document[] documents)
        {
            if (documents == null || documents.Length == 0) return;

            DocumentViewModel docVM;
            foreach (Models.Document doc in documents)
            {
                docVM = new DocumentViewModel(doc);
                Documents.Add(docVM);
                SelectedDocument = docVM;
            }
        }
        #endregion

        #region Rearranging docs/pages
        public void CopyDocumentTo(ViewModels.DocumentViewModel source, ViewModels.DocumentViewModel target)
        {
            if (source == null || target == null) return;

            foreach (ViewModels.PageViewModel page in source.Pages)
            {
                target.Pages.Add(PageViewModel.MakeCopy(page));
            }
        }

        public void MovePage(int oldIndex, int newIndex)
        {
            SelectedDocument.Pages.Move(oldIndex, newIndex);
        }

        public void MoveDocument(int oldIndex, int newIndex)
        {
            Documents.Move(oldIndex, newIndex);
        }

        public void MovePageToDoc(ViewModels.PageViewModel page, ViewModels.DocumentViewModel oldDoc, ViewModels.DocumentViewModel newDoc)
        {
            if (!oldDoc.Pages.Contains(page))
                throw new KeyNotFoundException(page + " not contained within " + oldDoc.Pages);
            if (null == oldDoc)
                throw new NullReferenceException("oldDoc");
            if (null == newDoc)
                throw new NullReferenceException("newDoc");

            oldDoc.Pages.Remove(page);
            newDoc.Pages.Add(page);
        }

        public void SplitDocument(ViewModels.DocumentViewModel docVM, int splitInterval)
        {
            ViewModels.DocumentViewModel newDocVM;
            Models.Document newDoc;
            int docCount = 0;

            if (docVM == null || splitInterval <= 0) return;

            newDocVM = docVM;

            while (docVM.PageCount> splitInterval)
            {
                // current doc VM reached its goal page-count - start the next one...
                if (newDocVM.PageCount >= splitInterval)
                {
                    newDoc = _pageFactory.CreateDocument();

                    newDoc.fName = docVM.DocName;
                    //newDoc.Rename("." + (++docCount) + "-" + (newDoc.id), true);
                    newDoc.image = docVM.Pages[0].Image;

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
        #endregion
        
        #region Utils
        public bool IsValidDropItem(IDataObject data)
        {
            string[] files = data.GetData(DataFormats.FileDrop) as string[];
            return (
                (
                    data.GetDataPresent(typeof(DocumentViewModel)) ||
                    data.GetDataPresent(typeof(PageViewModel))// ||
                    //IO.FileIO.IsExtensionSupported(files)
                ));
        }

        private void RenameDoc(DocumentViewModel docVM, string newDocName, bool isNewSubDoc=false)
        {
            if (docVM == null)
                throw new ArgumentNullException("docVM");
            if (String.IsNullOrEmpty(newDocName))
                throw new ArgumentNullException("newDocName");

            if (isNewSubDoc)
            {
                bool isSubIDNew = false;
                string docName;
                string path;
                string ext;

                for (int subDocID = 1; !isSubIDNew; subDocID++)
                {
                    for (int i = 0; i < Documents.Count; i++)
                    {
                        docName = Path.GetFileNameWithoutExtension(Documents[i].DocName);
                        if (!docName.EndsWith("-" + subDocID.ToString()))
                        {
                            path = Path.GetDirectoryName(newDocName) + "\\";
                            ext = Path.GetExtension(newDocName);

                            newDocName = path + docName + "-" + subDocID.ToString() + ext;
                            isSubIDNew = true;
                            break;
                        }
                    }
                }
            }

            docVM.DocName = newDocName;
        }
        
        #endregion
    }
}
