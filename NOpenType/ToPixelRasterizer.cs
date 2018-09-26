using System;

namespace NRasterizer
{
    // TODO This is currently instanciated per glyph... should be new up per renderer?
    /// <summary>
    /// Converts from glyph point based values into pixel based ones
    /// </summary>
    /// <remarks>
    /// This is internal as it will only ever be used by the <see cref="Renderer"/> and should not be used by external systems.
    /// </remarks>
    internal class ToPixelRasterizer
    {
        private readonly IGlyphRasterizer _inner;
        private readonly double _x;
        private readonly double _y;
        private readonly double _xScalar;
        private readonly double _yScalar;
        private readonly int _scalingFactor;
        private readonly int _divider;

        public ToPixelRasterizer(int x, int y, int scalingFactor, int divider, IGlyphRasterizer inner, bool flipY = true)
        {
            if (flipY)
            {
                _xScalar = 1.0;
                _yScalar = -1.0;
                _x = x;
                _y = y + EmSquare.Size;
            }
            else
            {
                _xScalar = 1.0;
                _yScalar = 1.0;
                _x = x;
                _y = y;
            }
            _scalingFactor = scalingFactor;
            _divider = divider;
            _inner = inner;
        }

        private double X(double x)
        {
            return (_scalingFactor * (_x + _xScalar * x)); // / _divider;
        }
        private double Y(double y)
        {
            return (_scalingFactor * (_y + _yScalar * y)); // / _divider;
        }

        public void BeginRead(int countourCount)
        {
            _inner.BeginRead(countourCount);
        }

        public void EndRead(double escapementX, double escapementY)
        {
            _inner.EndRead(X(escapementX), Y(escapementY));
        }

        public void LineTo(double x, double y)
        {
            _inner.LineTo(X(x), Y(y));
        }

        public void Curve3(double p2x, double p2y, double x, double y)
        {
            _inner.Curve3(X(p2x), Y(p2y), X(x), Y(y));
        }

        public void Curve4(double p2x, double p2y, double p3x, double p3y, double x, double y)
        {
            _inner.Curve4(
                X(p2x), Y(p2y),
                X(p3x), Y(p3y),
                X(x), Y(y)
            );
        }

        public void MoveTo(double x, double y)
        {
            _inner.MoveTo(X(x), Y(y));
        }

        public void CloseFigure()
        {
            _inner.CloseFigure();
        }
    }
}
