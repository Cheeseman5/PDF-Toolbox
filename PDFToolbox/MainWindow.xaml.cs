using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PDFToolbox.ViewModels;
using PDFToolbox.Models;

//using PdfSharp.Pdf;

namespace PDFToolbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private Helpers.DragDropHandler.Data _docsDropData;
        private Helpers.DragDropHandler.Data _pagesDropData;
        private Helpers.Toolbox _toolbox;
        private Helpers.FileIO _fileIO;

        public MainWindow(Helpers.Toolbox toolbox, Helpers.FileIO fileIO)
        {
            InitializeComponent();
            _toolbox = toolbox;
            _fileIO = fileIO;

            Title = _toolbox.AppCaption;

            _viewModel = this.DataContext as MainViewModel;
            _toolbox.WireSelectNameOnFocus(tbxDocumentName);
            _docsDropData = new Helpers.DragDropHandler.Data();
            _pagesDropData = new Helpers.DragDropHandler.Data();
            _toolbox.CreateLocalSaveDir();
        }

        private void lbxPages_Drop(object sender, DragEventArgs e)
        {
            HitTestResult hit = VisualTreeHelper.HitTest(sender as ListBox, e.GetPosition(sender as ListBox));

            // DraggedItem is a pageDict -> rearrange
            if (e.Data.GetDataPresent(typeof(ViewModels.PageViewModel)))
            {
                PageViewModel draggedPage = e.Data.GetData(typeof(PageViewModel)) as PageViewModel;
                ListBoxItem lbxItemDropTarget = _toolbox.FindParent<ListBoxItem>(hit.VisualHit);
                
                MovePage(draggedPage, lbxItemDropTarget);
                return;
            }

            // Get any files dropped onto pageview
            Document[] dropFiles = _fileIO.ExtractDocument(e.Data);

            // If any files dropped, load their pages
            if (dropFiles != null && dropFiles.Length > 0)
            {
                _viewModel.CachePages(dropFiles);
                return;
            }
        }

        private void MovePage(PageViewModel draggedPage, ListBoxItem lbxItemDropTarget)
        {
            // Move pageDict to last element if dropped on blank-space
            if (lbxItemDropTarget == null)
            {
                _viewModel.MovePage(
                    _viewModel.SelectedDocument.GetPageIndex(draggedPage),
                    _viewModel.Pages.Count - 1);
                return;
            }

            PageViewModel targetPage = lbxItemDropTarget.DataContext as ViewModels.PageViewModel;
            _viewModel.MovePage(
                _viewModel.SelectedDocument.GetPageIndex(draggedPage),
                _viewModel.SelectedDocument.GetPageIndex(targetPage));
            
        }

        private void lbxDocuments_Drop(object sender, DragEventArgs e)
        {
            // Existing doc/pageDict drop - move item
            HitTestResult hit = VisualTreeHelper.HitTest(sender as ListBox, e.GetPosition(sender as ListBox));
            ListBoxItem lbxItemDropTarget = _toolbox.FindParent<ListBoxItem>(hit.VisualHit);
            DocumentViewModel targetDoc = null;

            if (lbxItemDropTarget != null)
            {
                targetDoc = lbxItemDropTarget.DataContext as DocumentViewModel;
            }

            DocumentViewModel draggedDoc = e.Data.GetData(typeof(DocumentViewModel)) as DocumentViewModel;
            // When a doc is dropped
            if (draggedDoc != null)
            {
                bool ctrlKeyDown = e.KeyStates.HasFlag(DragDropKeyStates.ControlKey);
                MoveDocument(ctrlKeyDown, lbxItemDropTarget, targetDoc, draggedDoc);
                return;
            }

            PageViewModel pageViewModel = e.Data.GetData(typeof(PageViewModel)) as PageViewModel;
            // When a pageDict is dropped
            if (pageViewModel != null && targetDoc != null)
            {
                _viewModel.MovePageToDoc(pageViewModel,
                    _viewModel.SelectedDocument,
                    targetDoc);
                return;
            }

            // File-Drop - Load files
            Document[] dropFiles = _fileIO.ExtractDocument(e.Data);
            LoadFiles(dropFiles);
        }

        private void LoadFiles(Document[] dropFiles)
        {
            if (dropFiles == null || dropFiles?.Length == 0)
            {
                return;
            }

            _viewModel.CacheDocuments(dropFiles);
        }

        private void MoveDocument(bool ctrlKeyDown, ListBoxItem lbxItemDropTarget, DocumentViewModel targetDoc, DocumentViewModel draggedDoc)
        {
            if (lbxItemDropTarget == null)
            {
                _viewModel.MoveDocument(_viewModel.Documents.IndexOf(draggedDoc), _viewModel.Documents.Count - 1);
                return;
            }
            // If CTRL key is down -> copy pages to targetPdf doc
            if (ctrlKeyDown)
            {
                _viewModel.CopyDocumentTo(draggedDoc, targetDoc);
                return;
            }
            // CTRL not down -> move docs
            _viewModel.MoveDocument(_viewModel.Documents.IndexOf(draggedDoc), _viewModel.Documents.IndexOf(targetDoc));
            return;
        }

        private void lbxDocuments_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void lbxPages_DragEnter(object sender, DragEventArgs e)
        {
            bool validType = _viewModel.IsValidDropItem(e.Data);
            
            if (!validType)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void lbxPages_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _pagesDropData.DragStart = e.GetPosition(null);
        }

        private void lbxDocuments_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _docsDropData.DragStart = e.GetPosition(null);
        }

        private void lbxDocuments_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListBoxItem lbxItem = _toolbox.FindParent<ListBoxItem>((DependencyObject)e.OriginalSource);
            bool isDragging = Helpers.DragDropHandler.IsDragging(_docsDropData, e);

            if (!isDragging || lbxItem == null)
            {
                return;
            }

            ListBox lBox = sender as ListBox;
            DocumentViewModel doc = (DocumentViewModel)lBox.ItemContainerGenerator.ItemFromContainer(lbxItem);
            DataObject dataObj = new DataObject(typeof(DocumentViewModel), doc);

            DragDrop.DoDragDrop(lbxItem, dataObj, DragDropEffects.Move | DragDropEffects.Copy);
        }

        private void lbxPages_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListBoxItem lbxItem = _toolbox.FindParent<ListBoxItem>((DependencyObject)e.OriginalSource);
            ListBox lBox = sender as ListBox;
            bool isDragging = Helpers.DragDropHandler.IsDragging(_pagesDropData, e);

            if (!isDragging || lbxItem == null || lBox == null)
            {
                return;
            }

            PageViewModel page = (PageViewModel)lBox.ItemContainerGenerator.ItemFromContainer(lbxItem);
            DataObject dataObj = new DataObject(typeof(PageViewModel), page);

            try
            {
                DragDrop.DoDragDrop(lbxItem, dataObj, DragDropEffects.Move);
            }
            catch (Exception exception)
            {
                _toolbox.MessageBoxException(exception);
            }
        }

        private void winMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_fileIO.Cleanup();
        }

        private void PageEditCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.AddTextToggleButton?.IsChecked == false || _viewModel.SelectedPage == null)
            {
                return;
            }

            Point mouse = e.GetPosition(this.PageEditItemsControl);
            Canvas canvas = sender as Canvas;
            Point canvasOrigin = new Point();
            canvasOrigin.Y = (this.PageEditItemsControl.ActualHeight / 2) - (canvas.Height / 2);

            if (canvas.Width <= this.PageEditItemsControl.ActualWidth)
            {
                canvasOrigin.X = (this.PageEditItemsControl.ActualWidth / 2) - (canvas.Width / 2);
            }

            mouse.X = Math.Abs(mouse.X - canvasOrigin.X);
            mouse.Y = Math.Abs(mouse.Y - canvasOrigin.Y);

            AddTextAtMouse(mouse, this.AddTextTextBox.Text);
        }

        private void AddTextAtMouse(Point mouse, string text)
        {
            var txt = new TextBlock();
            var str = new UIString();
            var frmt = new FormattedText(text,
                                    CultureInfo.CurrentUICulture,
                                    FlowDirection.LeftToRight,
                                    new Typeface(txt.FontFamily, txt.FontStyle, txt.FontWeight, txt.FontStretch),
                                    txt.FontSize,
                                    Brushes.Black);
            str.String = text;
            str.X = mouse.X - (frmt.Width / 2);
            str.Y = mouse.Y - (frmt.Height / 2);
            str.Width = frmt.Width;
            str.Height = frmt.Height;

            _viewModel.SelectedPage.Strings.Add(str);
        }

        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Visible;
            InputBox.IsOpen = true;

            InputTextBox.Focus();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            string text = InputTextBox.Text;
            if (!InputTextBox_IsTextAllowed(text))
            {
                return;
            }

            _viewModel.SplitDocument(_viewModel.SelectedDocument, int.Parse(text));
            
            InputBox.Visibility = Visibility.Collapsed;
            InputBox.IsOpen = false;

            InputTextBox.Text = String.Empty;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Collapsed;
            InputBox.IsOpen = false;
            InputTextBox.Text = String.Empty;
        }

        private void InputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !InputTextBox_IsTextAllowed(e.Text);
        }

        private bool InputTextBox_IsTextAllowed(string text)
        {
            return Regex.IsMatch(text, "[0-9]");
        }
    }
}
