using System;
using OpenVG;

namespace Shapes
{
    public class Ellipse : Shape
    {
        public Ellipse(IOpenVG vg, float w, float h) : base(vg)
        {
            this.Bounds = new Bounds(w, h);
            vg.Ellipse(this.path, 0, 0, w, h);
        }

        public Bounds Bounds { get; }
    }
}
