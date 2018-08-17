using System;
using OpenVG;

namespace Shapes
{
    public class PaintColor : Paint
    {
        public PaintColor(IOpenVG vg, float[] color) : base(vg)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            if (color.Length != 4)
            {
                throw new ArgumentOutOfRangeException("color");
            }

            vg.SetParameteri(paint, (int)PaintParamType.VG_PAINT_TYPE, (int)PaintType.VG_PAINT_TYPE_COLOR);
            vg.SetParameterfv(paint, (int)PaintParamType.VG_PAINT_COLOR, color);
        }
    }
}