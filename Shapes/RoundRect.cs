using System;
using OpenVG;

namespace Shapes
{
    public class RoundRect : Shape
    {
        public RoundRect(IOpenVG vg, float x0, float y0, float width, float height, float arcWidth, float arcHeight)
            : base(vg)
        {
            vg.RoundRect(path, x0, y0, width, height, arcWidth, arcHeight);
        }
    }
}