﻿using System;
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
        public VAlign TextVAlign { get; set; }
        public HAlign TextHAlign { get; set; }

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
                var alignment = TranslateAlignment(TextHAlign, TextVAlign, lastTextBounds * TextSize);

                // Translate to alignment point:
                vg.PushMatrix();
                vg.Translate(alignment.X, alignment.Y);
                // Draw text:
                vg.FillPaint = TextColor;
                vg.DrawText(TextFont, lastTextLength, lastTextUTF32, PaintMode.VG_FILL_PATH, false, TextSize);
                vg.PopMatrix();
            }
        }
    }
}
