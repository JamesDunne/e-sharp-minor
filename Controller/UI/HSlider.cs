using System;
using Shapes;

namespace EMinor.UI
{
    public class HSlider : Component
    {
        private Bounds trackBounds;
        private Point trackPoint;
        private RoundRect trackRect;
        private Bounds handleBounds;
        private Point handlePoint;
        private RoundRect handleRect;

        public PaintColor Stroke { get; set; }
        public PaintColor Fill { get; set; }

        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public Func<float> Value { get; set; }

        public HSlider(IPlatform platform) : base(platform)
        {
            MinValue = 0.0f;
            MaxValue = 1.0f;
        }

        protected override void CreateShape()
        {
            if (handleRect != null)
            {
                handleRect.Dispose();
                handleRect = null;
            }

            handleBounds = new Bounds(12, 24);
            handlePoint = TranslateAlignment(HAlign.Left, VAlign.Middle, handleBounds);
            handleRect = new RoundRect(vg, handleBounds, 6, 6);

            if (trackRect != null)
            {
                trackRect.Dispose();
                trackRect = null;
            }

            trackBounds = new Bounds(Bounds.W - handleBounds.W * 2, 6);
            trackPoint = TranslateAlignment(HAlign.Center, VAlign.Middle, trackBounds);
            trackRect = new RoundRect(vg, trackBounds, 6, 6);
        }

        protected override void RenderSelf()
        {
            vg.StrokePaint = Stroke;
            vg.FillPaint = Fill;

            vg.PushMatrix();
            vg.Translate(trackPoint.X, trackPoint.Y);
            trackRect.Render(OpenVG.PaintMode.VG_FILL_PATH | OpenVG.PaintMode.VG_STROKE_PATH);
            vg.PopMatrix();

            vg.PushMatrix();
            float value = Value();
            float tx = (value / MaxValue) * (Bounds.W - handleBounds.W) - (handleBounds.W * 0.5f);
            vg.Translate(handlePoint.X + tx, handlePoint.Y);
            handleRect.Render(OpenVG.PaintMode.VG_FILL_PATH | OpenVG.PaintMode.VG_STROKE_PATH);
            vg.PopMatrix();
        }
    }
}
