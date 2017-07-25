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
            //LinearGradientBrush br = new LinearGradientBrush(new Point(0, 0), new Point(10, 10), Color.Black, Color.Black);
            //ColorBlend cb = new ColorBlend();
            Color intColor = Color.Green;

            if (color == Pens.Blue)
            {
                intColor = Color.Blue;
            }
            else if (color == Pens.Yellow)
            {
                intColor = Color.Yellow;
            }
            else if (color == Pens.Red)
            {
                intColor = Color.Red;
            }

            //if (color != Pens.Green)
            //{
            //    br.InterpolationColors = cb;
            //}
            //br.RotateTransform(180);
            //var centerPoint = new Point(LocalPosition.X - (Convert.ToInt32(size / 2)), LocalPosition.Y - Convert.ToInt32((size / 2)));
            var centerPoint = new Point(LocalPosition.X / 2, LocalPosition.Y / 2);

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(new Rectangle(centerPoint, new Size(Convert.ToInt32(size), Convert.ToInt32(size))));

            PathGradientBrush pgb = new PathGradientBrush(gp);

            pgb.CenterPoint = centerPoint;
            pgb.CenterColor = intColor;
            pgb.SurroundColors = new Color[] { Color.Green };
            pgb.SetBlendTriangularShape(.5f, 1.0f);
            pgb.FocusScales = new PointF(0f, 0f);

            g.FillPath(pgb, gp);

            pgb.Dispose();
            gp.Dispose();



            //g.FillEllipse(br, new Rectangle(centerPoint, new Size(Convert.ToInt32(size), Convert.ToInt32(size))));
        }
    }
}