using System;
using OpenVG;

namespace Shapes
{
    public abstract class Paint : IDisposable
    {
        private readonly IOpenVG vg;
        protected readonly PaintHandle paint;

        protected Paint(IOpenVG vg)
        {
            this.vg = vg;
            this.paint = vg.CreatePaint();
        }

        public void Dispose()
        {
            vg.DestroyPaint(this.paint);
        }

        public static implicit operator PaintHandle(Paint paint)
        {
            return paint.paint;
        }

        public PaintMode? PaintModes
        {
            get;
            set;
        }
    }
}