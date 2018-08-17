using System;
using System.Collections.Generic;
using OpenVG;

namespace Shapes
{
    public abstract class Shape : IDisposable
    {
        protected readonly IOpenVG vg;
        protected readonly uint path;

        protected Shape(IOpenVG vg)
        {
            this.vg = vg;
            this.PaintModes = PaintMode.VG_STROKE_PATH;

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

        static Dictionary<string, ParamType> fieldParamTypes = new Dictionary<string, ParamType> {
            { nameof(StrokeLineWidth), ParamType.VG_STROKE_LINE_WIDTH },
        };

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
            setContextParam(StrokeLineWidth, fieldParamTypes[nameof(StrokeLineWidth)]);
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
