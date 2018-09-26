using System;
using System.Collections.Generic;
using OpenVG;

namespace Shapes
{
    public abstract class Shape : IDisposable
    {
        protected readonly IOpenVG vg;
        protected readonly PathHandle path;

        protected Shape(IOpenVG vg)
        {
            this.vg = vg;
            this.PaintModes = PaintMode.VG_STROKE_PATH;

            // Create an OpenVG path resource:
            this.path = vg.CreatePathStandardFloat();
        }

        public void Dispose()
        {
            // Destroy the OpenVG path resource:
            vg.DestroyPath(this.path);
        }

        protected void setContextParam(float? newValue, ParamType type)
        {
            if (!newValue.HasValue) return;

            float oldValue = vg.Getf(type);
            if (newValue.Value != oldValue)
            {
                vg.Setf(type, newValue.Value);
            }
        }

        protected virtual void setRenderState()
        {
            setContextParam(StrokeLineWidth, ParamType.VG_STROKE_LINE_WIDTH);
        }

        public void Render(PaintMode? paintModes)
        {
            setRenderState();
            vg.DrawPath(this.path, paintModes ?? this.PaintModes);
        }

        public PaintMode PaintModes
        {
            get;
            set;
        }

        public float? StrokeLineWidth
        {
            get;
            set;
        }
    }
}
