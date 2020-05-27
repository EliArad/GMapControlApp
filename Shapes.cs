using GMap.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapNET_Example
{
    public enum shapes { line, circle, rectangle};
    public class GMapPoint : GMap.NET.WindowsForms.GMapMarker
    {
        private PointLatLng point_;
        private float size_;
        private shapes shapeToDraw_;
        private Pen pen_;
        public PointLatLng Point
        {
            get
            {
                return point_;
            }
            set
            {
                point_ = value;
            }
        }
        public GMapPoint(PointLatLng p, int size, shapes shapetoDraw, Pen pen)
            : base(p)
        {
            point_ = p;
            size_ = size;
            shapeToDraw_ = shapetoDraw;
            pen_ = pen;
        }

        public override void OnRender(Graphics g)
        {
            if (this.shapeToDraw_ == shapes.line)
            {
                g.DrawLine(this.pen_, LocalPosition.X, LocalPosition.Y, size_, size_);
            }
            if (this.shapeToDraw_ == shapes.circle)
            {
                g.DrawEllipse(this.pen_, LocalPosition.X, LocalPosition.Y, size_, size_);
            }
            if (this.shapeToDraw_ == shapes.rectangle)
            {

                g.DrawRectangle(this.pen_, LocalPosition.X, LocalPosition.Y, size_, size_);
            }

         

        }
    }
}
