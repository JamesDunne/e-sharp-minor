using System;
using OpenVG;

namespace Shapes
{
    public class Paint : IDisposable
    {
        private readonly IOpenVG vg;
        protected readonly PaintMode paintMode;
        protected readonly uint paint;

        public Paint(IOpenVG vg, PaintMode paintMode)
        {
            this.vg = vg;
            this.paintMode = paintMode;
            this.paint = vg.CreatePaint();
        }

        public void Dispose()
        {
            vg.DestroyPaint(this.paint);
        }

        public void Activate()
        {
            vg.SetPaint(paint, paintMode);
        }
    }
}