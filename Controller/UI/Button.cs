using System;
using EMinor;
using OpenVG;
using Shapes;

namespace EMinor.UI
{
    public class Button : Component
    {
        private RoundRect rect;
        private readonly float arcWidth;
        private readonly float arcHeight;

        public PaintColor Stroke { get; set; }
        public PaintColor Fill { get; set; }

        public Button(IPlatform platform, float arcWidth = 16.0f, float arcHeight = 16.0f)
            : base(platform)
        {
            this.arcWidth = arcWidth;
            this.arcHeight = arcHeight;
        }

        public override void CalculateChildrenLayout()
        {
            this.rect = new RoundRect(vg, this.Bounds, arcWidth, arcHeight);

            foreach (var child in Children)
            {
                child.ComputedPoint = new Point(arcWidth * 0.5f, arcHeight * 0.5f);
                child.ComputedBounds = new Bounds(Bounds.W - arcWidth, Bounds.H - arcHeight);
                child.CalculateChildrenLayout();
            }
        }

        public override void RenderSelf()
        {
            vg.StrokePaint = Stroke;
            vg.FillPaint = Fill;
            rect.Render(PaintMode.VG_STROKE_PATH | PaintMode.VG_FILL_PATH);
        }
    }
}
