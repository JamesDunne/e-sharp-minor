using System;
using System.Collections.Generic;
using Shapes;
using OpenVG;

namespace EMinor.UI
{
    public class HSlider : Component
    {
        private Bounds trackBounds;
        private Point trackPoint;
        private RoundRect trackRect;
        private Point handlePoint;
        private RoundRect handleRect;

        public PaintColor Stroke { get; set; }
        public PaintColor Fill { get; set; }

        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public Func<float> Value { get; set; }

        private readonly HSliderHandle handle;

        public HSlider(IPlatform platform) : base(platform)
        {
            MinValue = 0.0f;
            MaxValue = 1.0f;

            handle = new HSliderHandle(platform)
            {
                ComputedBounds = new Bounds(12, 24)
            };
            handle.SetParent(this);
            children.Add(handle);
        }

        public override IList<Component> Children
        {
            get => this.children;
            set => throw new NotSupportedException();
        }

        protected override void CreateShape()
        {
            if (handleRect != null)
            {
                handleRect.Dispose();
                handleRect = null;
            }

            handlePoint = TranslateAlignment(HAlign.Left, VAlign.Middle, handle.Bounds);

            if (trackRect != null)
            {
                trackRect.Dispose();
                trackRect = null;
            }

            trackBounds = new Bounds(Bounds.W - handle.Bounds.W * 2, 6);
            trackPoint = TranslateAlignment(HAlign.Center, VAlign.Middle, trackBounds);
            trackRect = new RoundRect(vg, trackBounds, 6, 6);
        }

        protected override void CalculateChildrenLayout(Point point, Bounds bounds, List<Component> fillChildren)
        {
            float value = Value();
            float tx = ((value - MinValue) / (MaxValue - MinValue)) * (Bounds.W - handle.Bounds.W) - (handle.Bounds.W * 0.5f);

            // Move the track handle according to computed horizontal position based on value, minvalue, maxvalue:
            handle.ComputedPoint = new Point(handlePoint.X + tx, handlePoint.Y);
        }

        protected override void RenderSelf()
        {
            vg.StrokePaint = Stroke;
            vg.FillPaint = Fill;

            vg.PushMatrix();
            vg.Translate(trackPoint.X, trackPoint.Y);
            trackRect.Render(PaintMode.VG_FILL_PATH | PaintMode.VG_STROKE_PATH);
            vg.PopMatrix();
        }

        private class HSliderHandle : Component
        {
            private RoundRect handleRect;

            private HSlider Slider => (HSlider)Parent;

            public PaintColor Stroke => Slider.Stroke;
            public PaintColor Fill => Slider.Fill;

            public HSliderHandle(IPlatform platform) : base(platform)
            {
            }

            protected override void CreateShape()
            {
                if (handleRect != null)
                {
                    handleRect.Dispose();
                    handleRect = null;
                }

                handleRect = new RoundRect(vg, Bounds, 6, 6);
            }

            protected override void RenderSelf()
            {
                vg.StrokePaint = Stroke;
                vg.FillPaint = Fill;

                handleRect.Render(OpenVG.PaintMode.VG_FILL_PATH | OpenVG.PaintMode.VG_STROKE_PATH);
            }
        }
    }
}
