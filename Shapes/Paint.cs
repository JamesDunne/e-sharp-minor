using System;
using OpenVG;

namespace Shapes
{
    public abstract class Paint : IDisposable
    {
        private readonly IOpenVG vg;
        protected readonly uint paint;

        protected Paint(IOpenVG vg)
        {
            this.vg = vg;
            this.paint = vg.CreatePaint();
        }

        public void Dispose()
        {
            vg.DestroyPaint(this.paint);
        }

        public void Activate(PaintMode? paintModes)
        {
            vg.SetPaint(paint, paintModes ?? this.PaintModes ?? PaintMode.VG_STROKE_PATH);
        }

        public PaintMode? PaintModes
        {
            get;
            set;
        }
    }
}