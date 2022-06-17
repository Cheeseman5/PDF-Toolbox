using PDFToolbox.Interfaces;
using PDFToolbox.Interfaces.Helpers;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace PDFToolbox.Helpers
{
    public class Toolbox : IToolbox
    {
        private IConfig _config;
        private string _defaultSaveLocation;
        private string _appVersion;
        private string _messageboxCaption;
        private string _messageboxExceptionCaption;
        private bool _silenceMessagebox;
        private string _messageboxExceptionMessage;
        public string AppCaption { get; private set; }

        public Toolbox(IConfig config)
        {
            _config = config;
            _defaultSaveLocation = _config.DefaultSaveDirectory;
            _appVersion = _config.AppVersion;
            _messageboxCaption = _config.AppName;
            _messageboxExceptionCaption = _messageboxCaption + ": Error";
            _silenceMessagebox = _config.SilenceMessagebox;
            _messageboxExceptionMessage = "An error occured in {0}:\n{1}\n\nStack trace:\n{2}\n\n{3}";

            AppCaption = $"{_config.AppName} - {_appVersion}";
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
                    System.Windows.Controls.TextBox tbx = FindParent<System.Windows.Controls.TextBox>((DependencyObject)sender);

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
            if (!_silenceMessagebox)
                System.Windows.Forms.MessageBox.Show(msg, _messageboxCaption);
        }
        public void MessageBoxException(Exception e, string addlMsg = "")
        {
            if (!_silenceMessagebox)
                System.Windows.Forms.MessageBox.Show(string.Format(_messageboxExceptionMessage, e.TargetSite, e.Message, e.StackTrace, addlMsg), _messageboxExceptionCaption);
        }

        #endregion
    }
}
