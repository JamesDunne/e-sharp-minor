using System;
using EMinor;
using OpenVG;
using Shapes;

namespace EMinor.UI
{
    public class Button : Component
    {
        private readonly RoundRect rect;

        public PaintColor Stroke { get; set; }
        public PaintColor Fill { get; set; }

        public Button(IPlatform platform, Bounds bounds)
            : base(platform, bounds)
        {
            disposalContainer.Add(
                rect = new RoundRect(vg, bounds.X, bounds.Y, bounds.W, bounds.H, 16, 16)
                {
                    StrokeLineWidth = 1.0f
                }
            );
        }

        public override void Render()
        {
            vg.StrokePaint = Stroke;
            vg.FillPaint = Fill;
            rect.Render(PaintMode.VG_STROKE_PATH | PaintMode.VG_FILL_PATH);
        }
    }
}
