using GMap.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEMOGUI
{
    public class GMapPoint : GMap.NET.WindowsForms.GMapMarker
    {
        private PointLatLng point;
        private float size;
        private Pen color;
        public PointLatLng Point
        {
            get
            {
                return this.point;
            }
            set
            {
                this.point = value;
            }
        }
        public GMapPoint(PointLatLng point, int size, Pen color)
            : base(point)
        {
            this.point = point;
            this.size = size;
            this.color = color;
        }

        public override void OnRender(Graphics g)
        {
            LinearGradientBrush br = new LinearGradientBrush(new Point(0, 0), new Point(10, 10), Color.Black, Color.Black);
            ColorBlend cb = new ColorBlend();

            if (color == Pens.Blue)
            {
                cb.Positions = new[] { 0f, 1 };
                cb.Colors = new[] { Color.Green, Color.Blue };
            }
            else if (color == Pens.Yellow)
            {
                cb.Positions = new[] { 0f, 0.5f, 1 };
                cb.Colors = new[] { Color.Green, Color.Blue, Color.Yellow };
            }
            else if (color == Pens.Red)
            {
                cb.Positions = new[] { 0, 1 / 3f, 2 / 3f, 1 };
                cb.Colors = new[] { Color.Green, Color.Yellow, Color.Orange, Color.Red };
            }
                        
            if (color != Pens.Green)
            {
                br.InterpolationColors = cb;
            }
            br.RotateTransform(180);
            var centerPoint = new Point(LocalPosition.X - (Convert.ToInt32(size / 2)), LocalPosition.Y - Convert.ToInt32((size / 1)));
            g.FillEllipse(br, new Rectangle(centerPoint, new Size(Convert.ToInt32(size), Convert.ToInt32(size))));
        }
    }
}