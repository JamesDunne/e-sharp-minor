using System;
using OpenVG;

namespace Shapes
{
    public abstract class Shape : IDisposable
    {
        protected readonly IOpenVG vg;
        protected readonly uint path;

        public Shape(IOpenVG vg)
        {
            this.vg = vg;

            // Create an OpenVG path resource:
            this.path = vg.CreatePath(
                Constants.VG_PATH_FORMAT_STANDARD,
                PathDatatype.VG_PATH_DATATYPE_F,
                1.0f,
                0.0f,
                0,
                0,
                PathCapabilities.VG_PATH_CAPABILITY_ALL
            );
        }

        public void Dispose()
        {
            // Destroy the OpenVG path resource:
            vg.DestroyPath(this.path);
        }

        public void Render(PaintMode? paintModes)
        {
            vg.DrawPath(this.path, paintModes ?? this.PaintModes ?? PaintMode.VG_STROKE_PATH);
        }

        public PaintMode? PaintModes {
            get;
            set;
        }
    }
}
