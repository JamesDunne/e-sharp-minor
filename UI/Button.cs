using System;
using e_sharp_minor;
using OpenVG;
using Shapes;

namespace UI
{
    public class Button : Component
    {
        private readonly RoundRect rect;

        public PaintColor Stroke { get; set; }
        public PaintColor Fill { get; set; }

        public Button(IPlatform platform, float x, float y, float width, float height)
            : base(platform, x, y, width, height)
        {
            disposalContainer.Add(
                rect = new RoundRect(vg, x, y, width, height, 16, 16)
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
