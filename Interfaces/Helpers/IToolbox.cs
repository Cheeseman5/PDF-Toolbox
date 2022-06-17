using System;
using System.Windows;

namespace PDFToolbox.Interfaces.Helpers
{
    public interface IToolbox
    {
        void CreateLocalSaveDir();
        T FindParent<T>(DependencyObject child) where T : DependencyObject;
        void WireSelectNameOnFocus(System.Windows.Controls.TextBox textBox);
        void SelectFileName(System.Windows.Controls.TextBox textbox);
        void MessageBox(string msg);
        void MessageBoxException(Exception e, string addlMsg = "");
    }
}
