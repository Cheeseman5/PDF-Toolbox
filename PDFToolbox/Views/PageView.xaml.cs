using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PDFToolbox.ViewModels;

namespace PDFToolbox.Views
{
    /// <summary>
    /// Interaction logic for PageView.xaml
    /// </summary>
    public partial class PageView : UserControl
    {
        //private PageViewModel _page;
        public PageViewModel PdfPage
        {
            get { return (PageViewModel)GetValue(PdfPageProperty); }
            set { SetValue(PdfPageProperty, value); }
        }

        public static readonly DependencyProperty PdfPageProperty = RegisterProperty<PageViewModel>("PdfPage");

        public PageView()
        {
            InitializeComponent();
            this.DataContext = PdfPage;
        }

        
        private static DependencyProperty RegisterProperty<T>(string varName, T defaultValue = default(T))
        {
            return DependencyProperty.Register(varName, typeof(T), typeof(PageView), new PropertyMetadata(defaultValue));
        }
    }
}
