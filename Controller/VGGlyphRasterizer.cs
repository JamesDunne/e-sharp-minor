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

        public void ConvertGlyphs(NRasterizer.Typeface typeFace, FontHandle destFont)
        {
            // Create a renderer instance that renders glyphs to OpenVG paths:
            var renderer = new NRasterizer.Renderer(typeFace, this);
            foreach (var c in typeFace.AllCharacters())
            {
                renderer.RenderChar(0, 0, c, 1, false);
                this.SetGlyphToPath(destFont, c);
            }
        }

        public void SetGlyphToPath(FontHandle font, uint glyphIndex)
        {
            PathHandle path = PathHandle.Invalid;
            if (segments.Count != 0)
            {
                byte[] segmentBytes = segments.Cast<byte>().ToArray();
                float[] coordsFloats = coords.ToArray();

                path = vg.CreatePath(
                    Constants.VG_PATH_FORMAT_STANDARD,
                    PathDatatype.VG_PATH_DATATYPE_F,
                    1.0f / 2048,
                    0.0f,
                    segmentBytes.Length,
                    coordsFloats.Length,
                    PathCapabilities.VG_PATH_CAPABILITY_ALL
                );

                vg.AppendPathData(path, segmentBytes, coordsFloats);
            }

            var origin = new float[] { 0.0f, 0.0f };
            vg.SetGlyphToPath(font, glyphIndex, path, false, origin, escapement);

            if (path != PathHandle.Invalid)
            {
                vg.DestroyPath(path);
            }
        }

        #region IGlyphRasterizer

        public int Resolution => 1;

        public void BeginRead(int countourCount)
        {
            this.segments = new List<PathSegment>(countourCount);
            this.coords = new List<float>(countourCount * 6);
        }

        public void EndRead(double escapementX, double escapementY)
        {
            this.escapement = new float[] { (float)escapementX / 2048, (float)escapementY / 2048 };
        }

        public void CloseFigure()
        {
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
        }

        #endregion
    }
}