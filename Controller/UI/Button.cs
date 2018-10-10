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

        public Func<string> Text { get; set; }
        public PaintColor TextColor { get; set; }
        public FontHandle TextFont { get; set; }

        public Button(IPlatform platform, Point point, RoundRect rect)
            : base(platform, point, rect.Bounds)
        {
            this.rect = rect;
        }

        public override void Render()
        {
            vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
            vg.PushMatrix();
            vg.Translate(point.X, point.Y);
            vg.StrokePaint = Stroke;
            vg.FillPaint = Fill;
            rect.Render(PaintMode.VG_STROKE_PATH | PaintMode.VG_FILL_PATH);
            vg.PopMatrix();

            if (Text != null)
            {
                vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE);
                vg.PushMatrix();
                // Left-aligned:
                vg.Translate(point.X + rect.ArcWidth * 0.5f, point.Y + rect.ArcHeight * 0.5f);
                vg.Scale(18, 18);
                vg.Setfv(ParamType.VG_GLYPH_ORIGIN, new float[] { 0.0f, 0.0f });
                vg.FillPaint = TextColor;
                vg.DrawGlyphs(TextFont, Text(), PaintMode.VG_FILL_PATH, false);
                vg.PopMatrix();
            }
        }
    }
}
