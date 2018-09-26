using System;
using OpenVG;

namespace EMinor
{
    public class VGGlyphRasterizer : NRasterizer.IGlyphRasterizer
    {
        private readonly IOpenVG vg;

        public VGGlyphRasterizer(IOpenVG vg)
        {
            this.vg = vg;
        }

        public int Resolution => 1;

        public void BeginRead(int countourCount)
        {
            //throw new NotImplementedException();
        }

        public void EndRead()
        {
            //throw new NotImplementedException();
        }

        public void CloseFigure()
        {
            //throw new NotImplementedException();
        }

        public void MoveTo(double x, double y)
        {
            //throw new NotImplementedException();
        }

        public void LineTo(double x, double y)
        {
            //throw new NotImplementedException();
        }

        public void Curve3(double p2x, double p2y, double x, double y)
        {
            //throw new NotImplementedException();
        }

        public void Curve4(double p2x, double p2y, double p3x, double p3y, double x, double y)
        {
            //throw new NotImplementedException();
        }

        public void Flush()
        {
            //throw new NotImplementedException();
        }
    }
}
