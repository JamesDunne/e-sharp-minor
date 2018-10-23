using System;
using System.Collections.Generic;
using NRasterizer;
using OpenVG;

namespace EMinor
{
    public class VGFont : IDisposable
    {
        private readonly IOpenVG vg;
        private readonly float Height;
        private FontHandle fontHandle;
        private Dictionary<uint, float[]> escapements;

        public VGFont(IOpenVG vg, FontHandle fontHandle, Dictionary<uint, float[]> escapements, float height)
        {
            this.vg = vg;
            this.fontHandle = fontHandle;
            this.escapements = escapements;
            Height = height;
        }

        public FontHandle FontHandle => fontHandle;
        public Dictionary<uint, float[]> Escapements => escapements;

        public static implicit operator FontHandle(VGFont font)
        {
            return font.fontHandle;
        }

        public Bounds MeasureText(string text)
        {
            //Console.WriteLine($"MeasureText('{text}')");
            float w = 0f;
            foreach (var ch in text)
            {
                w += escapements[ch][0];
            }
            return new Bounds(w, Height);
        }

        public void Dispose()
        {
            vg.DestroyFont(this.fontHandle);
            this.fontHandle = FontHandle.Invalid;
            this.escapements = null;
        }
    }
}
