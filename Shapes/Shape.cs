using System;
using OpenVG;

namespace Shapes
{
    public abstract class Shape : IDisposable
    {
        protected readonly IOpenVG vg;
        protected readonly uint path;
        protected readonly PaintMode paintModes;

        public Shape(IOpenVG vg, PaintMode paintModes)
        {
            this.vg = vg;
            this.paintModes = paintModes;

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
            if (this.path != 0)
            {
                // Destroy the OpenVG path resource:
                vg.DestroyPath(this.path);
            }
        }

        public abstract void ConstructPath();

        public void Render()
        {
            vg.DrawPath(this.path, paintModes);
        }
    }
}
