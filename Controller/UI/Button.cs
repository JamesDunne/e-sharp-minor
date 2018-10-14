using System;
using System.Collections.Generic;
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

        public float? StrokeLineWidth { get; set; }

        protected override void CreateShape()
        {
            this.rect = new RoundRect(vg, this.Bounds, arcWidth, arcHeight)
            {
                StrokeLineWidth = StrokeLineWidth
            };
        }

        protected override void CalculateChildrenLayout(Point point, Bounds bounds, List<Component> fillChildren)
        {
            foreach (var child in fillChildren)
            {
                child.ComputedPoint = new Point(arcWidth * 0.5f, arcHeight * 0.5f);
                child.ComputedBounds = new Bounds(bounds.W - arcWidth, bounds.H - arcHeight);
                child.CalculateLayout();
            }
        }

        protected override void RenderSelf()
        {
            if (Stroke != null)
            {
                vg.StrokePaint = Stroke;
            }
            if (Fill != null)
            {
                vg.FillPaint = Fill;
            }
            rect.Render(PaintMode.VG_STROKE_PATH | PaintMode.VG_FILL_PATH);
        }
    }
}
