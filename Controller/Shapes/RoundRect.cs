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
            ArcWidth = arcWidth;
            ArcHeight = arcHeight;
            vg.RoundRect(path, 0, 0, bounds.W - 1.0f, bounds.H - 1.0f, arcWidth, arcHeight);
        }

        public Bounds Bounds { get; }
        public float ArcWidth { get; }
        public float ArcHeight { get; }
    }
}