using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDFToolbox.Adorners
{
    public class PageViewFileDropAdorner : Adorner
    {
        private ContentPresenter _contentPresenter;
        private AdornerLayer _adornerLayer;
        private Size _offset;

        private Control _child;
        public Control Child
        {
            get { return _child; }
            set
            {
                if (_child != null)
                    RemoveVisualChild(_child);

                _child = value;

                if (_child != null)
                    AddVisualChild(_child);
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return (Child != null ? 1 : 0);
            }
        }

        #region Initialize
        public PageViewFileDropAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            /*Init(null, 
                null, 
                adornedElement, 
                AdornerLayer.GetAdornerLayer(adornedElement));*/
        }

        public PageViewFileDropAdorner(object data, DataTemplate dataTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            //Init(data, dataTemplate, adornedElement, adornerLayer);
        }

        protected void Init(object data, DataTemplate dataTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
        {
            _adornerLayer = adornerLayer;

            _contentPresenter = new ContentPresenter()
            {
                Content = data,
                ContentTemplate = dataTemplate,
                Opacity = 0.75
            };

            _adornerLayer.Add(this);
            _offset = new Size();
        }
        #endregion Initialize

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect size = new Rect(AdornedElement.RenderSize);
            drawingContext.DrawRoundedRectangle(Brushes.AliceBlue, null, size, 10, 10);
            drawingContext.DrawRoundedRectangle(
                Brushes.Transparent,
                new Pen(new SolidColorBrush(Color.FromScRgb(1f, 0.4f, 0.4f, 0.6f)), 5),
                new Rect(size.TopLeft.X + 10, size.TopLeft.Y + 10, size.Width - 20, size.Height - 20),
                10,
                10);
            //drawingContext.DrawText(Toolbox.Text.PAGE_VIEW_FILE_DROP_ADORNER, new Point(size.Width / 2, size.Height / 2));

            base.OnRender(drawingContext);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (Child == null)
                return constraint;

            Child.Measure(constraint);
            return Child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child == null)
                return finalSize;

            Child.Arrange(new Rect(new Point(0,0), finalSize));
            return new Size(Child.ActualWidth, Child.ActualHeight);
        }

        protected override Visual GetVisualChild(int index)
        {
            return Child;
        }


        public void UpdatePosition(double left, double top)
        {
            _offset.Width = left;
            _offset.Height = top;

            if (_adornerLayer != null)
                _adornerLayer.Update(this.AdornedElement);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_offset.Width, _offset.Height));

            return result;
        }

        public void Destroy()
        {
            _adornerLayer.Remove(this);
        }
    }
}
