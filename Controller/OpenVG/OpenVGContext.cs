using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OpenVG
{
    public class OpenVGContext : IOpenVG
    {
        internal OpenVGContext()
        {
        }

        #region VG

        const string vg = "OpenVG";

        [DllImport(vg, EntryPoint = "vgSeti")]
        extern static void vgSeti(ParamType paramType, int value);
        public void Seti(ParamType paramType, int value)
        {
            vgSeti(paramType, value);
        }

        [DllImport(vg, EntryPoint = "vgSetf")]
        extern static void vgSetf(ParamType paramType, float value);
        public void Setf(ParamType paramType, float value)
        {
            vgSetf(paramType, value);
        }

        [DllImport(vg, EntryPoint = "vgSetfv")]
        extern static void vgSetfv(ParamType paramType, int count, float[] values);
        public void Setfv(ParamType paramType, float[] values)
        {
            vgSetfv(paramType, values.Length, values);
        }

        [DllImport(vg, EntryPoint = "vgGeti")]
        extern static int vgGeti(ParamType type);
        public int Geti(ParamType type)
        {
            return vgGeti(type);
        }

        [DllImport(vg, EntryPoint = "vgGetf")]
        extern static float vgGetf(ParamType type);
        public float Getf(ParamType type)
        {
            return vgGetf(type);
        }

        [DllImport(vg, EntryPoint = "vgGetVectorSize")]
        extern static int vgGetVectorSize(ParamType type);
        [DllImport(vg, EntryPoint = "vgGetfv")]
        extern static void vgGetfv(ParamType type, int count, float[] values);
        public float[] Getfv(ParamType type)
        {
            int size = vgGetVectorSize(type);
            float[] vec = new float[size];
            vgGetfv(type, size, vec);
            return vec;
        }

        [DllImport(vg, EntryPoint = "vgGetiv")]
        extern static void vgGetiv(ParamType type, int count, int[] values);
        public int[] Getiv(ParamType type)
        {
            int size = vgGetVectorSize(type);
            int[] vec = new int[size];
            vgGetiv(type, size, vec);
            return vec;
        }

        [DllImport(vg, EntryPoint = "vgSetParameteri")]
        extern static void vgSetParameteri(uint handle, int paramType, int value);
        public void SetParameteri(uint handle, int paramType, int value)
        {
            vgSetParameteri(handle, paramType, value);
        }

        [DllImport(vg, EntryPoint = "vgSetParameterfv")]
        extern static void vgSetParameterfv(uint handle, int paramType, int count, float[] values);
        public void SetParameterfv(uint handle, int paramType, float[] values)
        {
            vgSetParameterfv(handle, paramType, values.Length, values);
        }

        [DllImport(vg, EntryPoint = "vgClear")]
        extern static void vgClear(int x, int y, int width, int height);
        public void Clear(int x, int y, int width, int height)
        {
            vgClear(x, y, width, height);
        }

        [DllImport(vg)]
        extern static void vgLoadIdentity();
        public void LoadIdentity()
        {
            vgLoadIdentity();
        }

        [DllImport(vg)]
        extern static void vgTranslate(float tx, float ty);
        public void Translate(float tx, float ty)
        {
            vgTranslate(tx, ty);
        }

        [DllImport(vg)]
        extern static void vgScale(float sx, float sy);
        public void Scale(float sx, float sy)
        {
            vgScale(sx, sy);
        }

        [DllImport(vg)]
        extern static void vgShear(float shx, float shy);
        public void Shear(float shx, float shy)
        {
            vgShear(shx, shy);
        }

        [DllImport(vg)]
        extern static void vgRotate(float angle);
        public void Rotate(float angle)
        {
            vgRotate(angle);
        }

        [DllImport(vg, EntryPoint = "vgCreatePath")]
        extern static uint vgCreatePath(int pathFormat,
                                PathDatatype datatype,
                                float scale, float bias,
                                int segmentCapacityHint,
                                int coordCapacityHint,
                                PathCapabilities capabilities);
        public PathHandle CreatePath(
            int pathFormat,
            PathDatatype datatype,
            float scale, float bias,
            int segmentCapacityHint,
            int coordCapacityHint,
            PathCapabilities capabilities
        )
        {
            return vgCreatePath(
                pathFormat,
                datatype,
                scale,
                bias,
                segmentCapacityHint,
                coordCapacityHint,
                capabilities
            );
        }

        public PathHandle CreatePathStandardFloat()
        {
            return vgCreatePath(
                Constants.VG_PATH_FORMAT_STANDARD,
                PathDatatype.VG_PATH_DATATYPE_F,
                1.0f,
                0.0f,
                0,
                0,
                PathCapabilities.VG_PATH_CAPABILITY_ALL
            );
        }

        [DllImport(vg, EntryPoint = "vgDestroyPath")]
        extern static void vgDestroyPath(uint path);
        public void DestroyPath(PathHandle path)
        {
            vgDestroyPath(path);
        }

        [DllImport(vg, EntryPoint = "vgDrawPath")]
        extern static void vgDrawPath(uint path, PaintMode paintModes);
        public void DrawPath(PathHandle path, PaintMode paintModes)
        {
            vgDrawPath(path, paintModes);
        }

        [DllImport(vg, EntryPoint = "vgAppendPathData")]
        extern static void vgAppendPathData(uint path, int numSegments, byte[] pathSegments, float[] pathData);
        public void AppendPathData(PathHandle path, byte[] segments, float[] coords)
        {
            vgAppendPathData(path, segments.Length, segments, coords);
        }

        [DllImport(vg, EntryPoint = "vgCreatePaint")]
        extern static uint vgCreatePaint();
        public PaintHandle CreatePaint()
        {
            return vgCreatePaint();
        }

        [DllImport(vg, EntryPoint = "vgDestroyPaint")]
        extern static void vgDestroyPaint(uint paint);
        public void DestroyPaint(PaintHandle paint)
        {
            vgDestroyPaint(paint);
        }

        [DllImport(vg, EntryPoint = "vgGetPaint")]
        extern static uint vgGetPaint(PaintMode paintModes);
        public PaintHandle GetPaint(PaintMode paintModes)
        {
            return vgGetPaint(paintModes);
        }

        [DllImport(vg, EntryPoint = "vgSetPaint")]
        extern static void vgSetPaint(uint paint, PaintMode paintModes);
        public void SetPaint(PaintHandle paint, PaintMode paintModes)
        {
            vgSetPaint(paint, paintModes);
        }

        [DllImport(vg, EntryPoint = "vgCreateFont")]
        extern static uint vgCreateFont(int glyphCapacityHint);
        public FontHandle CreateFont(int glyphCapacityHint)
        {
            return vgCreateFont(glyphCapacityHint);
        }

        [DllImport(vg, EntryPoint = "vgDestroyFont")]
        extern static uint vgDestroyFont(uint fontHandle);
        public void DestroyFont(FontHandle font)
        {
            vgDestroyFont(font);
        }

        [DllImport(vg, EntryPoint = "vgSetGlyphToPath")]
        extern static void vgSetGlyphToPath(uint font, uint glyphIndex, uint path, uint isHinted, float[] glyphOrigin, float[] escapement);
        public void SetGlyphToPath(FontHandle font, uint glyphIndex, PathHandle path, bool isHinted, float[] origin, float[] escapement)
        {
            vgSetGlyphToPath(font, glyphIndex, path, isHinted ? 1U : 0, origin, escapement);
        }

        [DllImport(vg, EntryPoint = "vgDrawGlyph")]
        extern static void vgDrawGlyph(uint font, uint glyphIndex, uint paintModes, uint allowAutoHinting);
        public void DrawGlyph(FontHandle font, uint glyphIndex, PaintMode paintModes, bool allowAutoHinting)
        {
            vgDrawGlyph(font, glyphIndex, (uint)paintModes, allowAutoHinting ? 1U : 0);
        }

        [DllImport(vg, EntryPoint = "vgDrawGlyphs")]
        extern static void vgDrawGlyphs(uint font, uint glyphCount, byte[] glyphIndices, float[] adjustmentsX, float[] adjustmentsY, uint paintModes, uint allowAutoHinting);
        public void DrawGlyphs(FontHandle font, string text, PaintMode paintModes, bool allowAutoHinting)
        {
            var glyphIndices = System.Text.Encoding.UTF32.GetBytes(text);
            vgDrawGlyphs(font, (uint)text.Length, glyphIndices, null, null, (uint)paintModes, allowAutoHinting ? 1U : 0);
        }

        #endregion

        #region VG Properties

        public float[] ClearColor
        {
            get
            {
                return Getfv(ParamType.VG_CLEAR_COLOR);
            }
            set
            {
                Setfv(ParamType.VG_CLEAR_COLOR, value);
            }
        }

        public PaintHandle StrokePaint
        {
            get
            {
                return GetPaint(PaintMode.VG_STROKE_PATH);
            }
            set
            {
                SetPaint(value, PaintMode.VG_STROKE_PATH);
            }
        }

        public PaintHandle FillPaint
        {
            get
            {
                return GetPaint(PaintMode.VG_FILL_PATH);
            }
            set
            {
                SetPaint(value, PaintMode.VG_FILL_PATH);
            }
        }

        #endregion

        #region VGU

        [DllImport(vg, EntryPoint = "vguLine")]
        extern static uint vgLine(uint path, float x0, float y0, float x1, float y1);
        public uint Line(PathHandle path, float x0, float y0, float x1, float y1)
        {
            return vgLine(path, x0, y0, x1, y1);
        }

        [DllImport(vg, EntryPoint = "vguRoundRect")]
        extern static uint vgRoundRect(uint path, float x, float y, float width, float height, float arcWidth, float arcHeight);
        public uint RoundRect(PathHandle path, float x, float y, float width, float height, float arcWidth, float arcHeight)
        {
            return vgRoundRect(path, x, y, width, height, arcWidth, arcHeight);
        }

        #endregion
    }
}
