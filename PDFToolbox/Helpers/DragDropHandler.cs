using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PDFToolbox.Helpers
{
    static class DragDropHandler
    {
        public struct Data
        {
            //public string DragFromName;
            //public FrameworkElement DraggedFromElement;
            public Point DragStart;
            //public object sender;
            //public MouseEventArgs mouse;
        }

        public static bool IsDragging(Data data, MouseEventArgs e)
        {
            return (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(data.DragStart.X - e.GetPosition(null).X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(data.DragStart.Y - e.GetPosition(null).Y) > SystemParameters.MinimumVerticalDragDistance));
        }


    }
}
