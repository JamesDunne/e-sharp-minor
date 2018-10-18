using System;
using NRasterizer;
using OpenVG;

namespace EMinor
{
    public class VGFont : IDisposable
    {
        private readonly IOpenVG vg;
        private FontHandle fontHandle;
        private Typeface typeFace;

        public VGFont(IOpenVG vg, FontHandle fontHandle, Typeface typeFace)
        {
            this.vg = vg;
            this.fontHandle = fontHandle;
            this.typeFace = typeFace;
        }

        public FontHandle FontHandle => fontHandle;
        public Typeface TypeFace => typeFace;

        public static implicit operator FontHandle(VGFont font)
        {
            return font.fontHandle;
        }

        public Bounds MeasureText(string text)
        {
            float w = 0f;
            foreach (var ch in text)
            {
                w += (float)typeFace.GetAdvanceWidth(ch) / 2048;
            }
            return new Bounds(w, 1.0f);
        }

        public void Dispose()
        {
            vg.DestroyFont(this.fontHandle);
            this.fontHandle = FontHandle.Invalid;
            this.typeFace = null;
        }
    }
}
