﻿using System;

namespace EMinor
{
    public struct Point
    {
        public float X;
        public float Y;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static readonly Point Zero = new Point(0, 0);

        public static Point operator +(in Point a, in Bounds b)
        {
            return new Point(a.X + b.W, a.Y + b.H);
        }

        public static Point operator +(in Point a, in Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(in Point a, in Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
