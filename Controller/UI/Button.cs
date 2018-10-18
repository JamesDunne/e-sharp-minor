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
        private bool pressed;
        private readonly float arcWidth;
        private readonly float arcHeight;

        public PaintColor Stroke { get; set; }
        public PaintColor Fill { get; set; }

        public PaintColor StrokePressed { get; set; }
        public PaintColor FillPressed { get; set; }

        public Button(IPlatform platform, float arcWidth = 16.0f, float arcHeight = 16.0f)
            : base(platform)
        {
            this.arcWidth = arcWidth;
            this.arcHeight = arcHeight;
            this.Padding = new Padding(arcHeight * 0.5f, arcWidth * 0.5f, arcHeight * 0.5f, arcWidth * 0.5f);
        }

        public override void Dispose()
        {
            if (this.rect != null)
            {
                this.rect.Dispose();
                this.rect = null;
            }
            base.Dispose();
        }

        public float? StrokeLineWidth { get; set; }

        protected override void CreateShape()
        {
            if (this.rect != null)
            {
                this.rect.Dispose();
            }

            float effLineWidth = (StrokeLineWidth ?? 1.0f) - 1.0f;
            this.rect = new RoundRect(vg, this.Bounds - new Bounds(effLineWidth, effLineWidth), arcWidth, arcHeight)
            {
                StrokeLineWidth = StrokeLineWidth
            };
        }

        protected override void RenderSelf()
        {
            if (pressed)
            {
                if (Stroke != null)
                {
                    vg.StrokePaint = StrokePressed ?? Stroke;
                }
                if (Fill != null)
                {
                    vg.FillPaint = FillPressed ?? Fill;
                }
            }
            else
            {
                if (Stroke != null)
                {
                    vg.StrokePaint = Stroke;
                }
                if (Fill != null)
                {
                    vg.FillPaint = Fill;
                }
            }
            float effLineWidth = (StrokeLineWidth ?? 1.0f) - 1.0f;
            vg.Translate(effLineWidth * 0.5f, effLineWidth * 0.5f);
            rect.Render(PaintMode.VG_STROKE_PATH | PaintMode.VG_FILL_PATH);
        }

        protected override void BeforeAction(in Point point, TouchAction action)
        {
            if (action == TouchAction.Pressed)
            {
                pressed = true;
            }
            else if (action == TouchAction.Released)
            {
                pressed = false;
            }
        }
    }
}
