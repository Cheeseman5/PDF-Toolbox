using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Controls;

using Media = System.Windows.Media;
using Controls = System.Windows.Controls;

namespace PDFToolbox
{
    [TemplatePart(Name = "PART_button", Type = typeof(Controls.Button))]
    [TemplatePart(Name = "PART_label", Type = typeof(Controls.Label))]
    [TemplatePart(Name = "PART_body", Type = typeof(ContentControl))]
    public partial class DocSnippet : System.Windows.Controls.Control
    {
        //----[ Properties ]------------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty BackgroundColorProperty = RegisterProperty<Color>("BackgroundColor", Color.AliceBlue);
        public static readonly DependencyProperty FooterTextProperty = RegisterProperty<string>("FooterText");


        public Color BackgroundColor
        {
            get { return (Color)this.GetValue(BackgroundColorProperty); }
            set { this.SetValue(BackgroundColorProperty, value); }
        }

        public string FooterText
        {
            get { return (string)this.GetValue(FooterTextProperty); }
            set { this.SetValue(FooterTextProperty, value); }
        }

        //----[ Controls ]-------------------------------------------------------------------------------------------------------------
        private Controls.Button Btn_Page;
        private Controls.Label Lbl_Footer;
        private ContentControl Body;

        //----[ Methods ]--------------------------------------------------------------------------------------------------------------
        public DocSnippet()
        {
            InitializeComponent();
        }

        //protected void Dispose(bool disposing) { }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.Template == null)
                return;

            Controls.Button btn_Page = this.Template.FindName("PART_button", this) as Controls.Button;

            if (btn_Page != Btn_Page)
            {
                if (Btn_Page != null)
                {
                    Btn_Page.MouseEnter -= new MouseEventHandler(Btn_Page_MouseEnter);
                    Btn_Page.MouseLeave -= new MouseEventHandler(Btn_Page_MouseLeave);
                }

                Btn_Page = btn_Page;

                if (Btn_Page != null)
                {
                    Btn_Page.MouseEnter += new MouseEventHandler(Btn_Page_MouseEnter);
                    Btn_Page.MouseLeave += new MouseEventHandler(Btn_Page_MouseLeave);
                }
            }

            Body = this.Template.FindName("PART_body", this) as ContentControl;
            Lbl_Footer = this.Template.FindName("PART_label", this) as Controls.Label;

        }

        private void Btn_Page_MouseEnter(object sender, MouseEventArgs e)
        {
            Controls.Button btn = sender as Controls.Button;

            if (btn == null)
                return;

            btn.Background = new Media.SolidColorBrush(Media.Colors.AliceBlue);
            if (Body != null)
            {
                //set Body foreground to white
            }
        }

        private void Btn_Page_MouseLeave(object sender, MouseEventArgs e)
        {
            Controls.Button btn = sender as Controls.Button;

            if (btn == null)
                return;
            btn.Background = new Media.SolidColorBrush(Media.Colors.Transparent);
            if (Body != null)
            {
                //set Body foreground to white
            }
        }



        /*private static DependencyProperty RegisterProperty<T>(string varName, T defaultValue)
        {
            return DependencyProperty.Register(varName, typeof(T), typeof(DocSnippet), new PropertyMetadata(defaultValue));
        }
        private static DependencyProperty RegisterProperty<T>(string varName)
        {
            return DependencyProperty.Register(varName, typeof(T), typeof(DocSnippet), new PropertyMetadata(default(T)));
        }*/
        private static DependencyProperty RegisterProperty<T>(string varName, T defaultValue = default(T))
        {
            return DependencyProperty.Register(varName, typeof(T), typeof(DocSnippet), new PropertyMetadata(defaultValue));
        }
    }
}
