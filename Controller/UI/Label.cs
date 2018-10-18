using System;
using OpenVG;
using Shapes;

namespace EMinor.UI
{
    public class Label : Component
    {
        public Func<string> Text { get; set; }
        public PaintColor TextColor { get; set; }
        public VGFont TextFont { get; set; }
        public float TextSize { get; set; }
        public TextVAlign TextVAlign { get; set; }
        public TextHAlign TextHAlign { get; set; }

        public string LastText { get; private set; }
        private byte[] lastTextUTF32;
        private Bounds lastTextBounds;
        private uint lastTextLength;

        public Label(IPlatform platform) : base(platform)
        {
            TextSize = 18.0f;
        }

        protected override void RenderSelf()
        {
            if (Text != null)
            {
                var text = Text();
                if (String.IsNullOrEmpty(text))
                {
                    LastText = text;
                    return;
                }

                // Only convert UTF32 when we need to:
                if (text != LastText)
                {
                    LastText = text;
                    lastTextLength = (uint)LastText.Length;
                    lastTextUTF32 = System.Text.Encoding.UTF32.GetBytes(LastText);
                    lastTextBounds = TextFont.MeasureText(LastText);
                }

                // Determine translation point from alignment settings:
                float tx = 0f;
                switch (TextHAlign)
                {
                    case TextHAlign.Left:
                        tx = 0f;
                        break;
                    case TextHAlign.Right:
                        tx = Bounds.W - lastTextBounds.W * TextSize;
                        break;
                    case TextHAlign.Center:
                        tx = Bounds.W * 0.5f - (lastTextBounds.W * 0.5f * TextSize);
                        break;
                }

                float ty = 0f;
                switch (TextVAlign)
                {
                    case TextVAlign.Bottom:
                        break;
                    case TextVAlign.Top:
                        ty = Bounds.H - lastTextBounds.H * TextSize;
                        break;
                    case TextVAlign.Middle:
                        ty = Bounds.H * 0.5f - (lastTextBounds.H * 0.5f * TextSize);
                        break;
                }

                vg.FillPaint = TextColor;

                // Translate to alignment point:
                vg.PushMatrix();
                vg.Translate(tx, ty);
                // Draw text:
                vg.DrawText(TextFont, lastTextLength, lastTextUTF32, PaintMode.VG_FILL_PATH, false, TextSize);
                vg.PopMatrix();
            }
        }
    }

    public enum TextVAlign
    {
        Bottom,
        Middle,
        Top
    }

    public enum TextHAlign
    {
        Left,
        Center,
        Right,
    }
}
