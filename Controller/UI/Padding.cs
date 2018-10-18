﻿namespace EMinor.UI
{
    public struct Padding
    {
        public float Top;
        public float Right;
        public float Bottom;
        public float Left;

        public Padding(float top, float right, float bottom, float left)
        {
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.Left = left;
        }

        public static readonly Padding Zero = new Padding(0, 0, 0, 0);
    }
}