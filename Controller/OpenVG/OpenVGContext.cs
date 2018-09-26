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

        [DllImport(vg, EntryPoint = "vgGetError")]
        extern static int vgGetError();
        public int GetError()
        {
            return vgGetError();
        }

        [DllImport(vg, EntryPoint = "vgSeti")]
        extern static void vgSeti(ParamType paramType, int value);
        public void Seti(ParamType paramType, int value)
        {
            vgSeti(paramType, value);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgSetf")]
        extern static void vgSetf(ParamType paramType, float value);
        public void Setf(ParamType paramType, float value)
        {
            vgSetf(paramType, value);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgSetfv")]
        extern static void vgSetfv(ParamType paramType, int count, float[] values);
        public void Setfv(ParamType paramType, float[] values)
        {
            vgSetfv(paramType, values.Length, values);
            checkError();
            return result;
        }

        [DllImport(vg, EntryPoint = "vgGeti")]
        extern static int vgGeti(ParamType type);
        public int Geti(ParamType type)
        {
            var result = vgGeti(type);
            checkError();
            return result;
        }

        [DllImport(vg, EntryPoint = "vgGetf")]
        extern static float vgGetf(ParamType type);
        public float Getf(ParamType type)
        {
            var result = vgGetf(type);
            checkError();
            return result;
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
            checkError();
            return vec;
        }

        [DllImport(vg, EntryPoint = "vgGetiv")]
        extern static void vgGetiv(ParamType type, int count, int[] values);
        public int[] Getiv(ParamType type)
        {
            int size = vgGetVectorSize(type);
            int[] vec = new int[size];
            vgGetiv(type, size, vec);
            checkError();
            return vec;
        }

        [DllImport(vg, EntryPoint = "vgSetParameteri")]
        extern static void vgSetParameteri(uint handle, int paramType, int value);
        public void SetParameteri(uint handle, int paramType, int value)
        {
            vgSetParameteri(handle, paramType, value);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgSetParameterfv")]
        extern static void vgSetParameterfv(uint handle, int paramType, int count, float[] values);
        public void SetParameterfv(uint handle, int paramType, float[] values)
        {
            vgSetParameterfv(handle, paramType, values.Length, values);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgClear")]
        extern static void vgClear(int x, int y, int width, int height);
        public void Clear(int x, int y, int width, int height)
        {
            vgClear(x, y, width, height);
            checkError();
        }

        [DllImport(vg)]
        extern static void vgLoadIdentity();
        public void LoadIdentity()
        {
            vgLoadIdentity();
            checkError();
        }

        [DllImport(vg)]
        extern static void vgTranslate(float tx, float ty);
        public void Translate(float tx, float ty)
        {
            vgTranslate(tx, ty);
            checkError();
        }

        [DllImport(vg)]
        extern static void vgScale(float sx, float sy);
        public void Scale(float sx, float sy)
        {
            vgScale(sx, sy);
            checkError();
        }

        [DllImport(vg)]
        extern static void vgShear(float shx, float shy);
        public void Shear(float shx, float shy)
        {
            vgShear(shx, shy);
            checkError();
        }

        [DllImport(vg)]
        extern static void vgRotate(float angle);
        public void Rotate(float angle)
        {
            vgRotate(angle);
            checkError();
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
            var result = vgCreatePath(
                pathFormat,
                datatype,
                scale,
                bias,
                segmentCapacityHint,
                coordCapacityHint,
                capabilities
            );
            checkError();
            return result;
        }

        public PathHandle CreatePathStandardFloat()
        {
            var result = vgCreatePath(
                Constants.VG_PATH_FORMAT_STANDARD,
                PathDatatype.VG_PATH_DATATYPE_F,
                1.0f,
                0.0f,
                0,
                0,
                PathCapabilities.VG_PATH_CAPABILITY_ALL
            );
            checkError();
            return result;
        }

        [DllImport(vg, EntryPoint = "vgDestroyPath")]
        extern static void vgDestroyPath(uint path);
        public void DestroyPath(PathHandle path)
        {
            vgDestroyPath(path);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgDrawPath")]
        extern static void vgDrawPath(uint path, PaintMode paintModes);
        public void DrawPath(PathHandle path, PaintMode paintModes)
        {
            vgDrawPath(path, paintModes);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgAppendPathData")]
        extern static void vgAppendPathData(uint path, int numSegments, byte[] pathSegments, float[] pathData);
        public void AppendPathData(PathHandle path, byte[] segments, float[] coords)
        {
            vgAppendPathData(path, segments.Length, segments, coords);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgCreatePaint")]
        extern static uint vgCreatePaint();
        public PaintHandle CreatePaint()
        {
            var result = vgCreatePaint();
            checkError();
            return result;
        }

        [DllImport(vg, EntryPoint = "vgDestroyPaint")]
        extern static void vgDestroyPaint(uint paint);
        public void DestroyPaint(PaintHandle paint)
        {
            vgDestroyPaint(paint);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgGetPaint")]
        extern static uint vgGetPaint(PaintMode paintModes);
        public PaintHandle GetPaint(PaintMode paintModes)
        {
            var result = vgGetPaint(paintModes);
            checkError();
            return result;
        }

        [DllImport(vg, EntryPoint = "vgSetPaint")]
        extern static void vgSetPaint(uint paint, PaintMode paintModes);
        public void SetPaint(PaintHandle paint, PaintMode paintModes)
        {
            vgSetPaint(paint, paintModes);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgCreateFont")]
        extern static uint vgCreateFont(int glyphCapacityHint);
        public FontHandle CreateFont(int glyphCapacityHint)
        {
            var result = vgCreateFont(glyphCapacityHint);
            checkError();
            return result;
        }

        [DllImport(vg, EntryPoint = "vgDestroyFont")]
        extern static uint vgDestroyFont(uint fontHandle);
        public void DestroyFont(FontHandle font)
        {
            vgDestroyFont(font);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgSetGlyphToPath")]
        extern static void vgSetGlyphToPath(uint font, uint glyphIndex, uint path, uint isHinted, float[] glyphOrigin, float[] escapement);
        public void SetGlyphToPath(FontHandle font, uint glyphIndex, PathHandle path, bool isHinted, float[] origin, float[] escapement)
        {
            vgSetGlyphToPath(font, glyphIndex, path, isHinted ? 1U : 0, origin, escapement);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgDrawGlyph")]
        extern static void vgDrawGlyph(uint font, uint glyphIndex, uint paintModes, uint allowAutoHinting);
        public void DrawGlyph(FontHandle font, uint glyphIndex, PaintMode paintModes, bool allowAutoHinting)
        {
            vgDrawGlyph(font, glyphIndex, (uint)paintModes, allowAutoHinting ? 1U : 0);
            checkError();
        }

        [DllImport(vg, EntryPoint = "vgDrawGlyphs")]
        extern static void vgDrawGlyphs(uint font, uint glyphCount, byte[] glyphIndices, float[] adjustmentsX, float[] adjustmentsY, uint paintModes, uint allowAutoHinting);
        public void DrawGlyphs(FontHandle font, string text, PaintMode paintModes, bool allowAutoHinting)
        {
            var glyphIndices = System.Text.Encoding.UTF32.GetBytes(text);
            vgDrawGlyphs(font, (uint)text.Length, glyphIndices, null, null, (uint)paintModes, allowAutoHinting ? 1U : 0);
            checkError();
        }

        private void checkError()
        {
            int err = GetError();
            if (err != 0)
            {
                throw new Exception(String.Format("VG error {0:X04}", err));
            }
        }

        #endregion

        #region VG Properties

        public float[] ClearColor
        {
            get
            {
                var color = Getfv(ParamType.VG_CLEAR_COLOR);
                checkError();
                return color;
            }
            set
            {
                Setfv(ParamType.VG_CLEAR_COLOR, value);
                checkError();
            }
        }

        public PaintHandle StrokePaint
        {
            get
            {
                var handle = GetPaint(PaintMode.VG_STROKE_PATH);
                checkError();
                return handle;
            }
            set
            {
                SetPaint(value, PaintMode.VG_STROKE_PATH);
                checkError();
            }
        }

        public PaintHandle FillPaint
        {
            get
            {
                var handle = GetPaint(PaintMode.VG_FILL_PATH);
                checkError();
                return handle;
            }
            set
            {
                SetPaint(value, PaintMode.VG_FILL_PATH);
                checkError();
            }
        }

        #endregion

        #region VGU

        [DllImport(vg, EntryPoint = "vguLine")]
        extern static uint vgLine(uint path, float x0, float y0, float x1, float y1);
        public uint Line(PathHandle path, float x0, float y0, float x1, float y1)
        {
            var result = vgLine(path, x0, y0, x1, y1);
            checkError();
            return result;
        }

        [DllImport(vg, EntryPoint = "vguRoundRect")]
        extern static uint vgRoundRect(uint path, float x, float y, float width, float height, float arcWidth, float arcHeight);
        public uint RoundRect(PathHandle path, float x, float y, float width, float height, float arcWidth, float arcHeight)
        {
            var result = vgRoundRect(path, x, y, width, height, arcWidth, arcHeight);
            checkError();
            return result;
        }

        #endregion
    }
}
