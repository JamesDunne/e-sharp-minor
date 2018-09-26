﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenVG;

namespace EMinor
{
    public class VGGlyphRasterizer : NRasterizer.IGlyphRasterizer
    {
        private readonly IOpenVG vg;

        private List<PathSegment> segments;
        private List<float> coords;
        private float[] escapement;

        public VGGlyphRasterizer(IOpenVG vg)
        {
            this.vg = vg;
        }

        public void SetGlyphToPath(FontHandle font, uint glyphIndex)
        {
            var path = vg.CreatePathStandardFloat();

            byte[] segmentBytes = segments.Cast<byte>().ToArray();
            float[] coordsFloats = coords.ToArray();
            vg.AppendPathData(path, segmentBytes, coordsFloats);

            var origin = new float[] { 0.0f, 0.0f };
            vg.SetGlyphToPath(font, glyphIndex, path, false, origin, escapement);

            vg.DestroyPath(path);
        }

        #region IGlyphRasterizer

        public int Resolution => 1;

        public void BeginRead(int countourCount)
        {
            this.segments = new List<PathSegment>(countourCount);
            this.coords = new List<float>(countourCount * 6);
        }

        public void EndRead()
        {
        }

        public void CloseFigure(double escapementX, double escapementY)
        {
            this.escapement = new float[] { (float)escapementX, (float)escapementY };
        }

        public void MoveTo(double x, double y)
        {
            segments.Add(PathSegment.VG_MOVE_TO);
            coords.Add((float)x);
            coords.Add((float)y);
        }

        public void LineTo(double x, double y)
        {
            segments.Add(PathSegment.VG_LINE_TO);
            coords.Add((float)x);
            coords.Add((float)y);
        }

        public void Curve3(double p2x, double p2y, double x, double y)
        {
            segments.Add(PathSegment.VG_QUAD_TO);
            coords.Add((float)p2x);
            coords.Add((float)p2y);
            coords.Add((float)x);
            coords.Add((float)y);
        }

        public void Curve4(double p2x, double p2y, double p3x, double p3y, double x, double y)
        {
            segments.Add(PathSegment.VG_CUBIC_TO);
            coords.Add((float)p2x);
            coords.Add((float)p2y);
            coords.Add((float)p3x);
            coords.Add((float)p3y);
            coords.Add((float)x);
            coords.Add((float)y);
        }

        public void Flush()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
