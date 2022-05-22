using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

//using PdfSharp.Pdf;
using PDFToolbox.IO;

namespace PDFToolbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModels.MainViewModel _viewModel;
        private Helpers.DragDropHandler.Data _docsDropData = new Helpers.DragDropHandler.Data();
        private Helpers.DragDropHandler.Data _pagesDropData = new Helpers.DragDropHandler.Data();
        private Adorners.PageViewFileDropAdorner _pageViewAdornerLayer;

        private Helpers.Toolbox _toolbox;
        private Helpers.FileIO _fileIO;

        public MainWindow()
        {
            InitializeComponent();

            Title = Toolbox.Info.APP_CAPTION;

            Toolbox.MainWindow = this;
            
            _pageViewAdornerLayer = new Adorners.PageViewFileDropAdorner(lbxPages);
            _viewModel = (ViewModels.MainViewModel)this.DataContext;
            Toolbox.WireSelectNameOnFocus(tbxDocumentName);

            Toolbox.CreateLocalSaveDir();

            // Register Extractors (pre-loaders)
            FileIO.RegisterStrategy(new OutlookAttachmentExtractor());
            FileIO.RegisterStrategy(new FileDropExtractor());
            // Register normal 
            FileIO.RegisterStrategy(new PdfFileIO());
        }

        public MainWindow(Helpers.Toolbox toolbox, Helpers.FileIO fileIO)
        {
            InitializeComponent();
            _toolbox = toolbox;
            _fileIO = fileIO;

            Title = _toolbox.info.APP_CAPTION;

            _pageViewAdornerLayer = new Adorners.PageViewFileDropAdorner(lbxPages);
            _viewModel = (ViewModels.MainViewModel)this.DataContext;
            _toolbox.WireSelectNameOnFocus(tbxDocumentName);

            _toolbox.CreateLocalSaveDir();

        }

        private void lbxPages_Drop(object sender, DragEventArgs e)
        {
            RemoveAdorner();
            
            HitTestResult hit = VisualTreeHelper.HitTest(sender as ListBox, e.GetPosition(sender as ListBox));

            // DraggedItem is a pageDict -> rearrange
            if (e.Data.GetDataPresent(typeof(ViewModels.PageViewModel)))
            {

                ViewModels.PageViewModel draggedPage = e.Data.GetData(typeof(ViewModels.PageViewModel)) as ViewModels.PageViewModel;
                ListBoxItem lbxItemDropTarget = Toolbox.FindParent<ListBoxItem>(hit.VisualHit);
                ViewModels.PageViewModel targetPage;

                // Move pageDict to last element if dropped on blank-space
                if (lbxItemDropTarget == null)
                {
                    _viewModel.MovePage(
                        _viewModel.SelectedDocument.GetPageIndex(draggedPage),
                        _viewModel.Pages.Count - 1);
                }
                else
                {
                    targetPage = lbxItemDropTarget.DataContext as ViewModels.PageViewModel;
                    _viewModel.MovePage(
                        _viewModel.SelectedDocument.GetPageIndex(draggedPage),
                        _viewModel.SelectedDocument.GetPageIndex(targetPage));
                }
                return;
            }

            // Get any files dropped onto pageview
            Models.Document[] dropFiles = _fileIO.ExtractDocument(e.Data);

            // If any files dropped, load their pages
            if (dropFiles != null && dropFiles.Length > 0)
            {
                _viewModel.CachePages(dropFiles);
                return;
            }
        }

        private void lbxDocuments_Drop(object sender, DragEventArgs e)
        {
            RemoveAdorner();

            // Existing doc/pageDict drop - move item
            HitTestResult hit = VisualTreeHelper.HitTest(sender as ListBox, e.GetPosition(sender as ListBox));
            ListBoxItem lbxItemDropTarget = Toolbox.FindParent<ListBoxItem>(hit.VisualHit);
            ViewModels.DocumentViewModel targetDoc = null;

            if (lbxItemDropTarget != null)
                targetDoc = lbxItemDropTarget.DataContext as ViewModels.DocumentViewModel;

            // When a doc is dropped
            if (e.Data.GetDataPresent(typeof(ViewModels.DocumentViewModel)))
            {
                ViewModels.DocumentViewModel draggedDoc = e.Data.GetData(typeof(ViewModels.DocumentViewModel)) as ViewModels.DocumentViewModel;

                if (lbxItemDropTarget == null)
                {
                    _viewModel.MoveDocument(_viewModel.Documents.IndexOf(draggedDoc), _viewModel.Documents.Count-1);
                    return;
                }
                else
                {
                    // If CTRL key is down -> copy pages to targetPdf doc
                    if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    {
                        _viewModel.CopyDocumentTo(draggedDoc, targetDoc);
                        return;
                    }
                    else // CTRL not down -> move docs
                    {
                        _viewModel.MoveDocument(_viewModel.Documents.IndexOf(draggedDoc), _viewModel.Documents.IndexOf(targetDoc));
                        return;
                    }
                }
            }

            // When a pageDict is dropped
            if(e.Data.GetDataPresent(typeof(ViewModels.PageViewModel)))
            {
                if (targetDoc != null)
                {
                    _viewModel.MovePageToDoc((ViewModels.PageViewModel)e.Data.GetData(typeof(ViewModels.PageViewModel)),
                        _viewModel.SelectedDocument,
                        targetDoc);
                }
                return;
            }

            // File-Drop - Load files
            Models.Document[] dropFiles = FileIO.ExtractDocument(e.Data);

            if (dropFiles != null && dropFiles.Length > 0)
            {
                _viewModel.CacheDocuments(dropFiles);
                return;
            }
        }

        private void lbxDocuments_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void lbxPages_DragEnter(object sender, DragEventArgs e)
        {
            bool validType = _viewModel.IsValidDropItem(e.Data);
            //Helpers.D.Log("{0} : {1} : {2}", validType, sender, e.Data.GetFormats());
            if (!validType)
            {
                e.Effects = DragDropEffects.None;
                /*_viewModel.DragOverPageViewValid = false;
                _viewModel.DragOverPageViewVisible = System.Windows.Visibility.Hidden;*/
            }
            else
            {
                /*AddAdorner();
                _viewModel.DragOverPageViewValid = true;
                _viewModel.DragOverPageViewVisible = System.Windows.Visibility.Visible;*/
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
            DataObject dataObj;
            ListBoxItem lbxItem;
            ListBox lBox;
            ViewModels.DocumentViewModel doc;

            if (!Helpers.DragDropHandler.IsDragging(_docsDropData, e))
                return;

            lbxItem = Toolbox.FindParent<ListBoxItem>((DependencyObject)e.OriginalSource);

            if (null == lbxItem)
                return;

            lBox = sender as ListBox;
            doc = (ViewModels.DocumentViewModel)lBox.ItemContainerGenerator.ItemFromContainer(lbxItem);
            dataObj = new DataObject(typeof(ViewModels.DocumentViewModel), doc);

            DragDrop.DoDragDrop(lbxItem, dataObj, DragDropEffects.Move | DragDropEffects.Copy);
        }

        private void lbxPages_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            DataObject dataObj;
            ListBoxItem lbxItem;
            ListBox lBox;
            ViewModels.PageViewModel page;
            
            if (!Helpers.DragDropHandler.IsDragging(_pagesDropData, e))
                return;

            lbxItem = Toolbox.FindParent<ListBoxItem>((DependencyObject)e.OriginalSource);

            if (lbxItem == null)
                return;

            lBox = sender as ListBox;
            if (lBox != null)
            {
                page = (ViewModels.PageViewModel)lBox.ItemContainerGenerator.ItemFromContainer(lbxItem);
                dataObj = new DataObject(typeof(ViewModels.PageViewModel), page);

                try
                {
                    DragDrop.DoDragDrop(lbxItem, dataObj, DragDropEffects.Move);
                }
                catch (Exception exception)
                {
                    Toolbox.MessageBoxException(exception);
                }
            }
        }

        private void tbxDocumentName_MouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox box = Toolbox.FindParent<TextBox>(e.OriginalSource as DependencyObject);

            if (box == null)
                return;

            if (!box.IsKeyboardFocusWithin)
            {
                box.Focus();
                e.Handled = true;
            }
        }

        private void tbxDocumentName_GotFocus(object sender, RoutedEventArgs e)
        {
            
            SelectDocName(e.OriginalSource as TextBox);
        }

        private void SelectDocName(TextBox textBox)
        {
            Toolbox.SelectFileName(textBox);

            /*if (textBox != null)
                textBox.Select((int)selection.X, (int)selection.Y);*/

        }

        private void lbiPage_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            //Helpers.D.Log(e.OriginalSource);
            //Helpers.D.Log(sender);
            //_viewModel.EditPage = Toolbox.FindParent<ViewModels.PageViewModel>((DependencyObject)e.OriginalSource);
        }

        private void AddAdorner()
        {
            /*AdornerLayer adornLayer = AdornerLayer.GetAdornerLayer(lbxPages);
            if (adornLayer != null)
                adornLayer.Add(_pageViewAdornerLayer);*/
        }

        private void RemoveAdorner()
        {
            /*AdornerLayer adornLayer = AdornerLayer.GetAdornerLayer(lbxPages);
            if (adornLayer != null)
                adornLayer.Remove(_pageViewAdornerLayer);*/
        }

        private void winMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IO.FileIO.Cleanup();
        }

        
        /*
        private void PageEdit_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void AddTextToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.PageEditCanvas != null)
            {
                TextBlock txt;

                txt = new TextBlock();
                txt.Foreground = new SolidColorBrush(Colors.Black);
                txt.Text = "Testing...";
                Canvas.SetTop(txt, 100);
                Canvas.SetRight(txt, 50);
                PageEditCanvas.Children.Add(txt);

                //this.AddTextToggleButton.IsChecked = false;
            }

            this.AddTextToggleButton.IsChecked = !this.AddTextToggleButton.IsChecked;
        }*/

        private void PageEditCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mouse;
            Point canvasOrigin;
            Canvas canvas;
            Models.UIString str;
            TextBlock txt;
            FormattedText frmt;

            if (this.AddTextToggleButton != null && this.AddTextToggleButton.IsChecked==true)
            {
                if (_viewModel.SelectedPage != null)
                {
                    mouse = e.GetPosition(this.PageEditItemsControl);
                    canvas = sender as Canvas;
                    canvasOrigin = new Point((canvas.Width > this.PageEditItemsControl.ActualWidth) ? (0) : (this.PageEditItemsControl.ActualWidth / 2) - (canvas.Width / 2),
                                            (this.PageEditItemsControl.ActualHeight / 2) - (canvas.Height / 2));
                    
                    txt = new TextBlock();
                    str = new Models.UIString();

                    str.String = this.AddTextTextBox.Text;

                    frmt = new FormattedText(str.String,
                                            CultureInfo.CurrentUICulture,
                                            FlowDirection.LeftToRight,
                                            new Typeface(txt.FontFamily, txt.FontStyle, txt.FontWeight, txt.FontStretch),
                                            txt.FontSize,
                                            Brushes.Black);
                    
                    str.X = Math.Abs(mouse.X - canvasOrigin.X) - (frmt.Width / 2);
                    str.Y = Math.Abs(mouse.Y - canvasOrigin.Y) - (frmt.Height / 2);
                    str.Width = frmt.Width;
                    str.Height = frmt.Height;

                    _viewModel.SelectedPage.Strings.Add(str);

                    txt = null;
                }
                //this.AddTextToggleButton.IsChecked = false;
            }
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
            //Helpers.D.Log("InputTextBox_IsTextAllowed(text):" + InputTextBox_IsTextAllowed(text));
            if (InputTextBox_IsTextAllowed(text))
            {
                _viewModel.SplitDocument(_viewModel.SelectedDocument, int.Parse(text));
                //Helpers.D.Log("I'm splitting!");
                InputBox.Visibility = Visibility.Collapsed;
                InputBox.IsOpen = false;

                InputTextBox.Text = String.Empty;
            }

        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Collapsed;
            InputBox.IsOpen = false;
            InputTextBox.Text = String.Empty;
        }

        private void InputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !InputTextBox_IsTextAllowed(e.Text); // InputTextBox_IsTextAllowed(e.Text);
        }

        private bool InputTextBox_IsTextAllowed(string text)
        {
            return Regex.IsMatch(text, "[0-9]");
        }
    }
}
