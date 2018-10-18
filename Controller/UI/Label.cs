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

        public string LastText { get; private set; }
        private byte[] lastUTF32Text;
        private uint lastUTF32Length;

        public Label(IPlatform platform) : base(platform)
        {
            TextSize = 18.0f;
        }

        protected override void RenderSelf()
        {
            if (Text != null)
            {
                var text = Text();
                if (text == null)
                {
                    LastText = null;
                    return;
                }

                // Only convert UTF32 when we need to:
                if (text != LastText)
                {
                    LastText = text;
                    lastUTF32Length = (uint)LastText.Length;
                    lastUTF32Text = System.Text.Encoding.UTF32.GetBytes(LastText);
                }

                vg.FillPaint = TextColor;
                vg.DrawText(TextFont, lastUTF32Length, lastUTF32Text, PaintMode.VG_FILL_PATH, false, TextSize);
            }
        }
    }
}
