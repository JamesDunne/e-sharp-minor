using System;

namespace EMinor.UI
{
    public struct Bounds
    {
        public float X;
        public float Y;
        public float W;
        public float H;

        public Bounds(float w, float h) {
            X = 0;
            Y = 0;
            W = w;
            H = h;
        }

        public Bounds(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
