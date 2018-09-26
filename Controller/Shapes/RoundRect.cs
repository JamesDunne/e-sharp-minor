using System;
using OpenVG;

namespace Shapes
{
    public class RoundRect : Shape
    {
        public RoundRect(IOpenVG vg, Bounds bounds, float arcWidth, float arcHeight)
            : base(vg)
        {
            this.Bounds = bounds;
            vg.RoundRect(path, 0, 0, bounds.W, bounds.H, arcWidth, arcHeight);
        }

        public Bounds Bounds { get; }
    }
}