using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace PDFToolbox.Helpers
{
    public class Toolbox
    {
        private string _defaultSaveLocation;
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

        // Re-enable this when config/settings get pulled in better
        //public Toolbox(string defaultSaveLocation)
        //{
        //    _defaultSaveLocation = defaultSaveLocation;
        //}
        // Remove this when the above ctor is enabled
        public void SetDefaultSaveLocation(string defaultSaveLocation)
        {
            _defaultSaveLocation = defaultSaveLocation;
        }

        #region File Handling

        public void CreateLocalSaveDir()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_defaultSaveLocation));
            }
            catch (Exception e)
            {
                string msg = $"An error occured when attempting to create directory \"{Path.GetDirectoryName(_defaultSaveLocation)}\"\n\n{e.Message}";
                MessageBox(msg);
            }
        }

        #endregion

        #region UX Utils
        public System.Windows.Controls.TextBox FindParent(System.Windows.Controls.TextBox child)
        {
            do
            {
                if (child is System.Windows.Controls.TextBox)
                    return child;
                child = child.Parent as System.Windows.Controls.TextBox;
            } while (child != null);
            return null;
        }

        // 'Wires up' a TextBox to select the file name when getting focus
        public void WireSelectNameOnFocus(System.Windows.Controls.TextBox textBox)
        {
            bool active = false;

            textBox.LostFocus += new RoutedEventHandler((sender, e) =>
                {
                    active = false;
                    // Remove selection when focus is lost
                    textBox.Select(0, 0);
                });

            textBox.MouseUp += new System.Windows.Input.MouseButtonEventHandler((sender, e) =>
                {
                    System.Windows.Controls.TextBox tbx = FindParent(sender as System.Windows.Controls.TextBox);

                    if (tbx == null)
                        return;

                    // If this is the first time clicking on the textbox...
                    if (!active)
                    {
                        // Leave selection alone if user selected text on first LeftMouseDown
                        //      (textbox gets focus while user selects part of its text)
                        if (textBox.SelectionLength == 0)
                        {
                            SelectFileName(textBox);
                            active = true;
                        }
                    }
                });
        }

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
        public void SelectFileName(System.Windows.Controls.TextBox textbox)
        {
            int start;
            int end;
            if (textbox != null)
            {
                FileNameTextSelection(textbox.Text, out start, out end);

                textbox.Select(start, end);
            }
        }
        #endregion

        #region Debugging
        public void MessageBox(string msg)
        {
            if (!debug.SILENCE_MESSAGEBOX)
                System.Windows.Forms.MessageBox.Show(msg, debug.MESSAGEBOX_CAPTION);
        }
        public void MessageBoxException(Exception e, string addlMsg = "")
        {
            if (!debug.SILENCE_MESSAGEBOX)
                System.Windows.Forms.MessageBox.Show(string.Format(debug.MESSAGEBOX_EXCEPTION_MESSAGE, e.TargetSite, e.Message, e.StackTrace, addlMsg), debug.MESSAGEBOX_EXCEPTION_CAPTION);
        }

        #endregion
    }
}
