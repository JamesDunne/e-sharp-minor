using System;
using OpenVG;
using Shapes;

namespace EMinor.UI
{
    public class Label : Component
    {
        public Func<string> Text { get; set; }
        public PaintColor TextColor { get; set; }
        public FontHandle TextFont { get; set; }
        public float TextSize { get; set; }

        public Label(IPlatform platform) : base(platform)
        {
            TextSize = 18.0f;
        }

        public override void RenderSelf()
        {
            if (Text != null)
            {
                vg.FillPaint = TextColor;
                vg.DrawText(TextFont, Text(), PaintMode.VG_FILL_PATH, false, TextSize);
            }
        }

        public override void CalculateChildrenLayout()
        {
            // No children.
        }
    }
}
