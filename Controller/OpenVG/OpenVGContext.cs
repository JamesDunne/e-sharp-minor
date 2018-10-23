#define FIXEDSTACK

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EMinor;

namespace OpenVG
{
    public class OpenVGContext : IOpenVG
    {
        internal OpenVGContext()
        {
        }

        #region VG

        const string vg = "AmanithVG";

        [DllImport(vg, EntryPoint = "vgGetError")]
        extern static int vgGetError();
        public int GetError()
        {
            return vgGetError();
        }

        [DllImport(vg, EntryPoint = "vgFlush")]
        extern static void vgFlush();
        public void Flush()
        {
            vgFlush();
        }

        [DllImport(vg, EntryPoint = "vgFinish")]
        extern static void vgFinish();
        public void Finish()
        {
            vgFinish();
        }

        [DllImport(vg, EntryPoint = "vgSeti")]
        extern static void vgSeti(ParamType paramType, int value);
        public void Seti(ParamType paramType, int value)
        {
            vgSeti(paramType, value);
            if (paramType == ParamType.VG_MATRIX_MODE)
            {
                matrixMode = value;
            }
        }

        [DllImport(vg, EntryPoint = "vgSetf")]
        extern static void vgSetf(ParamType paramType, float value);
        public void Setf(ParamType paramType, float value)
        {
            vgSetf(paramType, value);
        }

        [DllImport(vg, EntryPoint = "vgSetfv")]
        extern static unsafe void vgSetfv(ParamType paramType, int count, float* values);
        public unsafe void Setfv(ParamType paramType, float[] values)
        {
            fixed (float* p = values)
            {
                vgSetfv(paramType, values.Length, p);
            }
        }

        [DllImport(vg, EntryPoint = "vgGeti")]
        extern static int vgGeti(ParamType type);
        public int Geti(ParamType type)
        {
            var result = vgGeti(type);
            return result;
        }

        [DllImport(vg, EntryPoint = "vgGetf")]
        extern static float vgGetf(ParamType type);
        public float Getf(ParamType type)
        {
            var result = vgGetf(type);
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
        extern static unsafe void vgLoadMatrix(float* m);
        public unsafe void LoadMatrix(float[] m)
        {
            fixed (float* p = m)
            {
                vgLoadMatrix(p);
            }
        }

        [DllImport(vg)]
        extern static unsafe void vgGetMatrix(float* m);
        public unsafe void GetMatrix(float[] m)
        {
            fixed (float* p = m)
            {
                vgGetMatrix(p);
            }
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
            //Console.WriteLine("vgCreatePath");
            var result = vgCreatePath(
                pathFormat,
                datatype,
                scale,
                bias,
                segmentCapacityHint,
                coordCapacityHint,
                capabilities
            );
            return result;
        }

        public PathHandle CreatePathStandardFloat()
        {
            //Console.WriteLine("vgCreatePath");
            var result = vgCreatePath(
                Constants.VG_PATH_FORMAT_STANDARD,
                PathDatatype.VG_PATH_DATATYPE_F,
                1.0f,
                0.0f,
                0,
                0,
                PathCapabilities.VG_PATH_CAPABILITY_ALL
            );
            return result;
        }

        [DllImport(vg, EntryPoint = "vgDestroyPath")]
        extern static void vgDestroyPath(uint path);
        public void DestroyPath(PathHandle path)
        {
            //Console.WriteLine("vgDestroyPath");
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
            var result = vgCreatePaint();
            return result;
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
            var result = vgGetPaint(paintModes);
            return result;
        }

        [DllImport(vg, EntryPoint = "vgSetPaint")]
        extern static void vgSetPaint(uint paint, PaintMode paintModes);
        public void SetPaint(PaintHandle paint, PaintMode paintModes)
        {
            vgSetPaint(paint, paintModes);
        }

        [DllImport(vg)]
        extern static uint vgCreateImage(int format, int width, int height, int allowedQuality);
        public ImageHandle CreateImage(ImageFormat format, int width, int height, ImageQuality allowedQuality)
        {
            return vgCreateImage((int)format, width, height, (int)allowedQuality);
        }

        [DllImport(vg)]
        extern static void vgDestroyImage(uint image);
        public void DestroyImage(ImageHandle image)
        {
            vgDestroyImage(image);
        }

        [DllImport(vg)]
        extern static unsafe void vgImageSubData(uint image, void* data, int dataStride, int dataFormat, int x, int y, int width, int height);
        public unsafe void ImageSubData(ImageHandle image, void* data, int dataStride, ImageFormat dataFormat, int x, int y, int width, int height)
        {
            vgImageSubData(image, data, dataStride, (int)dataFormat, x, y, width, height);
        }

        [DllImport(vg)]
        extern static uint vgChildImage(uint parent, int x, int y, int width, int height);
        public ImageHandle ChildImage(ImageHandle parent, int x, int y, int width, int height)
        {
            return vgChildImage(parent, x, y, width, height);
        }

        [DllImport(vg)]
        extern static void vgDrawImage(uint image);
        public void DrawImage(ImageHandle image)
        {
            vgDrawImage(image);
        }

        [DllImport(vg, EntryPoint = "vgCreateFont")]
        extern static uint vgCreateFont(int glyphCapacityHint);
        public FontHandle CreateFont(int glyphCapacityHint)
        {
            var result = vgCreateFont(glyphCapacityHint);
            return result;
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

        [DllImport(vg)]
        extern static unsafe void vgSetGlyphToImage(uint font, uint glyphIndex, uint image, float* origin, float* escapement);
        public void SetGlyphToImage(FontHandle font, uint glyphIndex, ImageHandle image, float[] origin, float[] escapement)
        {
            unsafe
            {
                float* op = stackalloc float[2];
                float* ep = stackalloc float[2];
                op[0] = origin[0];
                op[1] = origin[1];
                ep[0] = escapement[0];
                ep[1] = escapement[1];
                vgSetGlyphToImage(font, glyphIndex, image, op, ep);
            }
        }

        [DllImport(vg, EntryPoint = "vgDrawGlyph")]
        extern static void vgDrawGlyph(uint font, uint glyphIndex, uint paintModes, uint allowAutoHinting);
        public void DrawGlyph(FontHandle font, uint glyphIndex, PaintMode paintModes, bool allowAutoHinting)
        {
            vgDrawGlyph(font, glyphIndex, (uint)paintModes, allowAutoHinting ? 1U : 0);
        }

        [DllImport(vg, EntryPoint = "vgDrawGlyphs")]
        extern static unsafe void vgDrawGlyphs(uint font, uint glyphCount, uint* glyphIndices, float[] adjustmentsX, float[] adjustmentsY, uint paintModes, uint allowAutoHinting);
        public unsafe void DrawGlyphs(FontHandle font, uint glyphCount, uint* glyphIndices, PaintMode paintModes, bool allowAutoHinting)
        {
            vgDrawGlyphs(font, glyphCount, glyphIndices, null, null, (uint)paintModes, allowAutoHinting ? 1U : 0);
        }

        public void DrawGlyphs(FontHandle font, uint[] glyphIndices, PaintMode paintModes, bool allowAutoHinting)
        {
            unsafe
            {
                fixed (uint* p = glyphIndices)
                {
                    vgDrawGlyphs(font, (uint)glyphIndices.Length, p, null, null, (uint)paintModes, allowAutoHinting ? 1U : 0);
                }
            }
        }

        public void DrawGlyphs(FontHandle font, uint glyphCount, byte[] utf32Text, PaintMode paintModes, bool allowAutoHinting)
        {
            unsafe
            {
                fixed (byte* bytes = utf32Text)
                {
                    vgDrawGlyphs(font, glyphCount, (uint*)bytes, null, null, (uint)paintModes, allowAutoHinting ? 1U : 0);
                }
            }
        }

        public void DrawGlyphString(FontHandle font, string text, PaintMode paintModes, bool allowAutoHinting)
        {
            unsafe
            {
                ReadOnlySpan<char> chars = text.AsSpan();
                int byteCount = System.Text.Encoding.UTF32.GetByteCount(chars);
                byte* bytes = stackalloc byte[byteCount];
                System.Text.Encoding.UTF32.GetBytes(chars, new Span<byte>(bytes, byteCount));
                vgDrawGlyphs(font, (uint)text.Length, (uint*)bytes, null, null, (uint)paintModes, allowAutoHinting ? 1U : 0);
            }
        }

        public void CheckError()
        {
            int err = GetError();
            if (err != 0)
            {
                throw new Exception(String.Format("VG error {0:X04}", err));
            }
        }

        #endregion

        #region VG fakes

#if FIXEDSTACK
        private unsafe struct MatrixStack
        {
            public uint Head;
            public fixed float Data[maxBytes];

            public const int MaxSize = 20;
            private const int maxBytes = 9 * MaxSize;

            public unsafe void Push(float* m)
            {
                if (Head >= maxBytes)
                {
                    throw new InvalidOperationException();
                }

                // Copy in new data:
                fixed (float* f = Data)
                {
                    float* p = f + Head;
                    for (int i = 0; i < 9; i++, p++, m++)
                    {
                        *p = *m;
                    }
                }
                Head += 9;
            }

            public unsafe float* Pop()
            {
                if (Head <= 0)
                {
                    throw new InvalidOperationException();
                }

                Head -= 9;
                //for (int i = 0; i < 9; i++)
                //{
                //    m[i] = Data[Head + i];
                //}

                fixed (float* f = Data)
                {
                    return &f[Head];
                }
            }
        }
#endif

        private class MatrixState
        {
#if FIXEDSTACK
            public MatrixStack Stack;
#else
            public Stack<float[]> Stack = new Stack<float[]>();
#endif
        }

        int matrixMode = (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE;
        readonly Dictionary<int, MatrixState> matrixState = new Dictionary<int, MatrixState>()
        {
            { (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE, new MatrixState() },
            { (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE, new MatrixState() },
        };

        public void PushMatrix()
        {
            MatrixState state = matrixState[matrixMode];

            unsafe
            {
#if FIXEDSTACK
                float* m = stackalloc float[9];
                vgGetMatrix(m);
                state.Stack.Push(m);
#else
                float[] m = new float[9];
                fixed (float* p = m)
                {
                    vgGetMatrix(p);
                }
                state.Stack.Push(m);
#endif
            }
        }

        public void PopMatrix()
        {
            MatrixState state = matrixState[matrixMode];

            unsafe
            {
#if FIXEDSTACK
                float* m = state.Stack.Pop();
                vgLoadMatrix(m);
#else
                float[] m = state.Stack.Pop();
                fixed (float* p = m)
                {
                    vgLoadMatrix(p);
                }
#endif
            }
        }

        public void DrawText(FontHandle textFont, string text, PaintMode paintModes, bool allowAutoHinting, float size)
        {
            unsafe
            {
                int mm = matrixMode;

                // Get current matrix:
                float* m = stackalloc float[9];
                vgGetMatrix(m);

                // Switch to glyph matrix if not already:
                if (mm != (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE)
                {
                    vgSeti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE);
                    vgLoadMatrix(m);
                }

                // Render text:
                //vgScale(size, size);

                // TODO: restore VG_GLYPH_ORIGIN afterwards?
                float* origin = stackalloc float[2];
                origin[0] = 0f;
                origin[1] = 0f;
                vgSetfv(ParamType.VG_GLYPH_ORIGIN, 2, origin);

                //var glyphIndices = System.Text.Encoding.UTF32.GetBytes(text);
                DrawGlyphString(textFont, text, paintModes, false);

                // Restore matrix mode:
                if (mm != (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE)
                {
                    // TODO: restore glyph matrix before switching back?
                    vgSeti(ParamType.VG_MATRIX_MODE, mm);
                }

                // Restore old matrix:
                vgLoadMatrix(m);
            }
        }

        public void DrawText(FontHandle textFont, uint glyphCount, byte[] utf32Text, PaintMode paintModes, bool allowAutoHinting, float size)
        {
            unsafe
            {
                int mm = matrixMode;

                // Get current matrix:
                float* m = stackalloc float[9];
                vgGetMatrix(m);

                // Switch to glyph matrix if not already:
                if (mm != (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE)
                {
                    vgSeti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE);
                    vgLoadMatrix(m);
                }

                // Render text:
                //vgScale(size, size);

                float* origin = stackalloc float[2];
                origin[0] = 0f;
                origin[1] = 0f;
                vgSetfv(ParamType.VG_GLYPH_ORIGIN, 2, origin);

                fixed (byte* bytes = utf32Text)
                {
                    // default:
                    vgDrawGlyphs(textFont, glyphCount, (uint*)bytes, null, null, (uint)paintModes, 0U);

                    // paintModes = 0 still renders glyphs!
                    //vgDrawGlyphs(textFont, glyphCount, (uint*)bytes, null, null, (uint)0, 0U);

                    //uint* p = (uint*)bytes;
                    //for (uint i = 0; i < glyphCount; i++, p++)
                    //{
                    //    vgDrawGlyph(textFont, *p++, (uint)paintModes, 0U);
                    //}
                }

                // Restore matrix mode:
                if (mm != (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE)
                {
                    // TODO: restore glyph matrix before switching back?
                    vgSeti(ParamType.VG_MATRIX_MODE, mm);
                }

                // Restore old matrix:
                vgLoadMatrix(m);
            }
        }

        public void DrawText(VGFont font, uint glyphCount, byte[] utf32Text, PaintMode paintModes, bool allowAutoHinting)
        {
            unsafe
            {
                int mm = matrixMode;

                // Get current matrix:
                float* m = stackalloc float[9];
                vgGetMatrix(m);

                // Switch to image matrix if not already:
                if (mm != (int)MatrixMode.VG_MATRIX_IMAGE_USER_TO_SURFACE)
                {
                    vgSeti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_IMAGE_USER_TO_SURFACE);
                    vgLoadMatrix(m);
                }

                //VG_MATRIX_IMAGE_USER_TO_SURFACE
                // Render text:
                fixed (byte* bytes = utf32Text)
                {
                    uint* p = (uint*)bytes;
                    for (uint i = 0; i < glyphCount; i++, p++)
                    {
                        //vgDrawGlyph(textFont, *p, (uint)paintModes, 0U);

                        // Translate to origin of glyph:
                        var origin = font.Origins[*p];
                        vgTranslate(-origin[0], -origin[1]);

                        // Draw the image:
                        vgDrawImage(font.Images[*p]);

                        // Undo origin translation:
                        vgTranslate(origin[0], origin[1]);

                        // Advance to next glyph position:
                        var escapement = font.Escapements[*p];
                        vgTranslate(escapement[0], escapement[1]);
                    }
                }

                // Restore previous matrix mode:
                if (mm != (int)MatrixMode.VG_MATRIX_IMAGE_USER_TO_SURFACE)
                {
                    vgSeti(ParamType.VG_MATRIX_MODE, mm);
                }

                // Restore old matrix:
                vgLoadMatrix(m);
            }
        }

        #endregion

        #region VG Properties

        public float[] ClearColor
        {
            get
            {
                var color = Getfv(ParamType.VG_CLEAR_COLOR);
                return color;
            }
            set
            {
                unsafe
                {
                    fixed (float* p = value)
                    {
                        vgSetfv(ParamType.VG_CLEAR_COLOR, value.Length, p);
                    }
                }
            }
        }

        private PaintHandle strokePaint;

        public PaintHandle StrokePaint
        {
            get
            {
                return strokePaint;
                //var handle = GetPaint(PaintMode.VG_STROKE_PATH);
                //return handle;
            }
            set
            {
                if (strokePaint == value) return;
                vgSetPaint(value, PaintMode.VG_STROKE_PATH);
                strokePaint = value;
            }
        }

        private PaintHandle fillPaint;

        public PaintHandle FillPaint
        {
            get
            {
                return fillPaint;
                //var handle = GetPaint(PaintMode.VG_FILL_PATH);
                //return handle;
            }
            set
            {
                if (fillPaint == value) return;
                vgSetPaint(value, PaintMode.VG_FILL_PATH);
                fillPaint = value;
            }
        }

        #endregion

        #region VGU

        [DllImport(vg, EntryPoint = "vguLine")]
        extern static uint vgLine(uint path, float x0, float y0, float x1, float y1);
        public uint Line(PathHandle path, float x0, float y0, float x1, float y1)
        {
            var result = vgLine(path, x0, y0, x1, y1);
            return result;
        }

        [DllImport(vg, EntryPoint = "vguRoundRect")]
        extern static uint vgRoundRect(uint path, float x, float y, float width, float height, float arcWidth, float arcHeight);
        public uint RoundRect(PathHandle path, float x, float y, float width, float height, float arcWidth, float arcHeight)
        {
            var result = vgRoundRect(path, x, y, width, height, arcWidth, arcHeight);
            return result;
        }

        [DllImport(vg, EntryPoint = "vguEllipse")]
        extern static uint vguEllipse(uint path, float cx, float cy, float width, float height);
        public uint Ellipse(PathHandle path, float cx, float cy, float width, float height)
        {
            var result = vguEllipse(path, cx, cy, width, height);
            return result;
        }

        #endregion
    }
}
