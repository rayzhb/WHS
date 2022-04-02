using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WHS.DEVICE.MAPDESIGN.Controls
{
    [Obsolete("方法过时")]
    public class GridRectangle : CustomRectangle
    {
        public Func<int, int, bool> DrawFunc;

        protected override void OnRender(DrawingContext drawingContext)
        {
         
            if (DrawFunc(base.RowIndex, base.ColumnIndex - 1))
            {
                //left
                drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(0, base.Height / 2), new Point(base.Width / 2, base.Height / 2));
            }
            if (DrawFunc(base.RowIndex, base.ColumnIndex+1))
            {
                //right
                drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(base.Width / 2, base.Height / 2), new Point(base.Width, base.Height / 2));

            }
            if (DrawFunc(base.RowIndex-1, base.ColumnIndex ))
            {
                //up
                drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(base.Width / 2, 0), new Point(base.Width / 2, base.Height / 2));

            }
            if (DrawFunc(base.RowIndex + 1, base.ColumnIndex))
            {
                //down
                drawingContext.DrawLine(new Pen(Brushes.Black, 1), new Point(base.Width / 2, base.Height / 2), new Point(base.Width / 2, base.Height));
            }
            //画椭圆
            drawingContext.DrawEllipse(Brushes.White, new Pen(Brushes.Black, 1), new Point(base.Width / 2, base.Height / 2), base.Height / 8, base.Height / 8);


            base.OnRender(drawingContext);
        }
    }
}
