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

        public Button(IPlatform platform, Point point, RoundRect rect)
            : base(platform, point, rect.Bounds)
        {
            this.rect = rect;
        }

        public override void Render()
        {
            vg.StrokePaint = Stroke;
            vg.FillPaint = Fill;
            vg.PushMatrix();
            vg.Translate(point.X, point.Y);
            rect.Render(PaintMode.VG_STROKE_PATH | PaintMode.VG_FILL_PATH);
            vg.PopMatrix();
        }
    }
}
