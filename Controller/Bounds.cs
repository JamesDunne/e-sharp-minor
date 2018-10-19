using System;

namespace EMinor
{
    public struct Bounds
    {
        public float W;
        public float H;

        public Bounds(float w, float h)
        {
            W = w;
            H = h;
        }

        public static Bounds operator +(Bounds a, Bounds b)
        {
            return new Bounds(a.W + b.W, a.H + b.H);
        }

        public static Bounds operator -(Bounds a, Bounds b)
        {
            return new Bounds(a.W - b.W, a.H - b.H);
        }

        public static Bounds operator *(Bounds b, float scalar)
        {
            return new Bounds(b.W * scalar, b.H * scalar);
        }
    }
}
