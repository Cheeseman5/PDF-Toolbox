using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PDFToolbox
{
    public class SmartWrapPanel : WrapPanel
    {
        public static readonly DependencyProperty ChildrenScaleProperty = RegisterProperty<double>("ChildrenScale", 1.0);


        public double ChildrenScale
        {
            get { return (double)this.GetValue(ChildrenScaleProperty); }
            set 
            {
                if (value != (double)this.GetValue(ChildrenScaleProperty))
                {
                    this.SetValue(ChildrenScaleProperty, value);
                    RecalculateSizing();
                }
            }
        }


        public SmartWrapPanel() : base()
        {
        }

        public void RecalculateSizing()
        {
            /*double maxChildWidth = 0;
            PDFPage page;   // PDFPage no longer exists
            double pageWidth = 0;

            if (Children.Count == 0)
                return;

            foreach (UIElement e in Children)
            {
                page = (PDFPage)e;
                pageWidth = page.Width * ChildrenScale;

                if (pageWidth > maxChildWidth)
                    maxChildWidth = pageWidth;
            }

            MinWidth = maxChildWidth;
            return;*/
        }

        private static DependencyProperty RegisterProperty<T>(string varName, T defaultValue = default(T))
        {
            return DependencyProperty.Register(varName, typeof(T), typeof(DocSnippet), new PropertyMetadata(defaultValue));
        }
    }
}
