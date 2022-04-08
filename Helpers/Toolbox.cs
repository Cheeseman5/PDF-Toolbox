using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Media;

//using iTextSharp.text.pdf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PDFToolbox.Helpers
{
    public class Toolbox
    {
        #region Helper Reference Classes
        public class Info
        {
            public readonly string APP_VERSION;
            public readonly string APP_CAPTION;

            public Info()
            {
                APP_VERSION = "a0.6.4";
                APP_CAPTION = "PDF Toolbox - " + APP_VERSION;
            }

        }
        public Info info { get; private set; } = new Info();
        public class Debug
        {
            public readonly bool SILENCE_MESSAGEBOX;
            public readonly string MESSAGEBOX_CAPTION;
            public readonly string MESSAGEBOX_EXCEPTION_CAPTION;
            public readonly string MESSAGEBOX_EXCEPTION_MESSAGE;

            public Debug()
            {
                SILENCE_MESSAGEBOX = false;
                MESSAGEBOX_CAPTION = "PDF Toolbox";
                MESSAGEBOX_EXCEPTION_CAPTION = MESSAGEBOX_CAPTION + ": Error";
                MESSAGEBOX_EXCEPTION_MESSAGE = "An error occured in {0}:\n{1}\n\nStack trace:\n{2}\n\n{3}";
            }
        }
        public Debug debug { get; private set; } = new Debug();
        #endregion

        #region Utils
        // Find parent of 'child' of type 'T'
        public T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            do
            {
                if (child is T)
                    return (T)child;
                child = VisualTreeHelper.GetParent(child);
            } while (child != null);
            return null;
        }


        #endregion

        #region File Handling

        public void RemoveTemporaryFiles()
        {
            //IO.FileIO.Cleanup();
        }

        public void CreateLocalSaveDir()
        {
            try
            {
                //Directory.CreateDirectory(Path.GetDirectoryName(IO.FileIO.SaveDirectoryDefault));
            }
            catch (Exception e)
            {
                //Toolbox.MessageBox("An error occured when attempting to create directory \"{0}\"\n\n{1}", Path.GetDirectoryName(IO.FileIO.SaveDirectoryDefault), e.Message);
            }
        }

        #endregion

        #region UX Utils

        public object MainWindow { get; set; }
        //public Window MainWindow { get; set; }

        // 'Wires up' a TextBox to select the file name when getting focus
        public void WireSelectNameOnFocus(object textBox) { }
        //public void WireSelectNameOnFocus(TextBox textBox)
        //{
        //    bool active = false;

        //    textBox.LostFocus += new RoutedEventHandler((sender, e) =>
        //        {
        //            active = false;
        //            // Remove selection when focus is lost
        //            textBox.Select(0, 0);
        //        });

        //    textBox.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler( (sender, e) =>
        //        {
        //            TextBox tbx = FindParent<TextBox>(sender as DependencyObject);

        //            if (tbx == null)
        //                return;

        //            // If this is the first time clicking on the textbox...
        //            if (!active)
        //            {
        //                // Leave selection alone if user selected text on first LeftMouseDown (textbox gets focus while user selects part of its text)
        //                if (textBox.SelectionLength == 0)
        //                {
        //                    SelectFileName(textBox);
        //                    active = true;
        //                }
        //            }
        //        });
        //}

        // Gets starting and ending points of filename within text
        private void FileNameTextSelection(string text, out int start, out int end)
        {
            if (string.IsNullOrEmpty(text))
            {
                start = 0;
                end = 0;
                return;
            }

            string name = Path.GetFileNameWithoutExtension(text);
            start = text.LastIndexOf(name);
            end = name.Length;
        }
        // Sets textbox's selection to the filename within, selects nothing if no filename found
        public void SelectFileName(object textbox) { }
        //public static void SelectFileName(TextBox textbox)
        //{
        //    int start;
        //    int end;
        //    if (textbox != null)
        //    {
        //        FileNameTextSelection(textbox.Text, out start, out end);

        //        textbox.Select(start, end);
        //    }
        //}
        #endregion

        #region Debugging
        public void MessageBox(string msg) { }
        //public void MessageBox(string msg)
        //{
        //    if(!Debug.SILENCE_MESSAGEBOX)
        //        System.Windows.MessageBox.Show(msg, Debug.MESSAGEBOX_CAPTION);
        //}
        public void MessageBox(string msg, params object[] args) { }
        //public void MessageBox(string msg, params object[] args)
        //{
        //    if (!Debug.SILENCE_MESSAGEBOX)
        //        System.Windows.MessageBox.Show(string.Format(msg, args), Debug.MESSAGEBOX_CAPTION);
        //}

        public void MessageBoxException(Exception e) { }
        //public void MessageBoxException(Exception e)
        //{
        //    if (!Debug.SILENCE_MESSAGEBOX)
        //        System.Windows.MessageBox.Show(string.Format(Debug.MESSAGEBOX_EXCEPTION_MESSAGE, e.TargetSite, e.Message, e.StackTrace, ""), Debug.MESSAGEBOX_EXCEPTION_CAPTION);
        //}
        public void MessageBoxException(Exception e, string addlMsg) { }
        //public void MessageBoxException(Exception e, string addlMsg)
        //{
        //    if (!Debug.SILENCE_MESSAGEBOX)
        //        System.Windows.MessageBox.Show(string.Format(Debug.MESSAGEBOX_EXCEPTION_MESSAGE, e.TargetSite, e.Message, e.StackTrace, addlMsg), Debug.MESSAGEBOX_EXCEPTION_CAPTION);
        //}

        #endregion
    }
}
