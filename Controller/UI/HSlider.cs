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

        public PaintColor Stroke { get; set; }
        public PaintColor Fill { get; set; }

        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public Func<float> Value { get; set; }
        public Action<float> ValueChanged { get; set; }

        public ActionHandler OnHandlePress { get => handle.OnPress; set => handle.OnPress = value; }
        public ActionHandler OnHandleMove { get => handle.OnMove; set => handle.OnMove = value; }
        public ActionHandler OnHandleRelease { get => handle.OnRelease; set => handle.OnRelease = value; }

        private readonly HSliderHandle handle;

        public HSlider(IPlatform platform) : base(platform)
        {
            MinValue = 0.0f;
            MaxValue = 1.0f;

            // Create handle child in constructor so that delegating property assignments will work:
            handle = new HSliderHandle(platform);
            handle.SetParent(this);
            handle.ComputedBounds = new Bounds(12, 24);
            children.Add(handle);
        }

        public override IList<Component> Children
        {
            get => this.children;
            set => throw new NotSupportedException();
        }

        protected override void CreateShape()
        {
            handlePoint = TranslateAlignment(HAlign.Left, VAlign.Middle, handle.Bounds);

            if (trackRect != null)
            {
                trackRect.Dispose();
                trackRect = null;
            }

            trackBounds = new Bounds(Bounds.W - handle.Bounds.W, 6);
            trackPoint = TranslateAlignment(HAlign.Center, VAlign.Middle, trackBounds);
            trackRect = new RoundRect(vg, trackBounds, 6, 6);
        }

        public override bool IsPressable => true;

        protected override void BeforeAction(in Point point, TouchAction action)
        {
            // Take a point relative to our root offset:
            var value = PointToValue(point);
            // Update the value:
            ValueChanged?.Invoke(value);
            LayoutCalculated = false;
        }

        protected override void CalculateChildrenLayout(Point point, Bounds bounds, List<Component> fillChildren)
        {
            float value = Value();
            float tx = ((value - MinValue) / (MaxValue - MinValue)) * (Bounds.W - handle.Bounds.W);

            // Move the track handle according to computed horizontal position based on value, minvalue, maxvalue:
            handle.ComputedPoint = new Point(tx, handlePoint.Y);
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

        protected float PointToValue(in Point absPoint)
        {
            var relPoint = absPoint - (RootOffset + trackPoint);
            var x = (relPoint.X + (handle.Bounds.W * 0.5f)) / trackBounds.W * (MaxValue - MinValue) + MinValue;
            if (x > MaxValue) x = MaxValue;
            if (x < MinValue) x = MinValue;
            return x;
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

            public override bool IsPressable => true;

            protected override void BeforeAction(in Point point, TouchAction action)
            {
                // Take a point relative to our parent's root offset:
                var value = Slider.PointToValue(point);
                // Update the value:
                Slider.ValueChanged?.Invoke(value);
                Slider.LayoutCalculated = false;
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
