using System;
using System.Collections.Generic;
using NRasterizer;
using OpenVG;

namespace EMinor
{
    public class VGFont : IDisposable
    {
        private readonly IOpenVG vg;
        private FontHandle fontHandle;
        private Dictionary<uint, float[]> escapements;

        public VGFont(IOpenVG vg, FontHandle fontHandle, Dictionary<uint, float[]> escapements)
        {
            this.vg = vg;
            this.fontHandle = fontHandle;
            this.escapements = escapements;
        }

        public FontHandle FontHandle => fontHandle;
        public Dictionary<uint, float[]> Escapements => escapements;

        public static implicit operator FontHandle(VGFont font)
        {
            return font.fontHandle;
        }

        public Bounds MeasureText(string text)
        {
            float w = 0f;
            foreach (var ch in text)
            {
                w += escapements[ch][0];
            }
            return new Bounds(w, 1.0f);
        }

        public void Dispose()
        {
            vg.DestroyFont(this.fontHandle);
            this.fontHandle = FontHandle.Invalid;
            this.escapements = null;
        }
    }
}
