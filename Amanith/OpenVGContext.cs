using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenVG;

namespace Amanith
{
    public class OpenVGContext : IDisposable, IOpenVG
    {
        internal readonly IntPtr vgContext;
        internal readonly IntPtr vgSurface;

        internal readonly Glfw.Monitor monitor;
        internal readonly Glfw.Window window;

        public OpenVGContext(int width, int height)
        {
            Debug.WriteLine("glfw.Init()");
            Glfw.Init();

            Debug.WriteLine("window = glfw.CreateWindow()");
            window = Glfw.CreateWindow(width, height, "e-sharp-minor");

            Debug.WriteLine("glfw.MakeContextCurrent(window)");
            Glfw.MakeContextCurrent(window);

            // Get the real framebuffer size for OpenGL pixels; should work with Retina:
            Glfw.GetFramebufferSize(window, out width, out height);
            this.Width = width;
            this.Height = height;

            // create an OpenVG context
            Debug.WriteLine("vgContext = vgPrivContextCreateAM(0)");
            vgContext = vgPrivContextCreateAM(IntPtr.Zero);

            // create a drawing surface (sRGBA premultiplied color space)
            vgSurface = vgPrivSurfaceCreateAM(this.Width, this.Height, 0, 1, 1);

            // bind context and surface
            vgPrivMakeCurrentAM(vgContext, vgSurface);

            Debug.WriteLine("glfw.ShowWindow(window)");
            Glfw.ShowWindow(window);
        }

        public void Dispose()
        {
            Glfw.HideWindow(window);

            // Destroy OpenVG context:
            Debug.WriteLine("vgPrivMakeCurrentAM(0, 0)");
            vgPrivMakeCurrentAM(IntPtr.Zero, IntPtr.Zero);

            Debug.WriteLine("vgPrivSurfaceDestroyAM(vgSurface)");
            vgPrivSurfaceDestroyAM(vgSurface);

            Debug.WriteLine("vgPrivContextDestroyAM(vgContext)");
            vgPrivContextDestroyAM(vgContext);

            // Tear down glfw:
            Debug.WriteLine("glfw.DestroyWindow(window)");
            Glfw.DestroyWindow(window);

            Debug.WriteLine("glfw.Terminate()");
            Glfw.Terminate();
        }

        const string vg = "AmanithVG";

        [DllImport(vg)]
        extern static IntPtr vgPrivContextCreateAM(IntPtr sharedContext);

        [DllImport(vg)]
        extern static void vgPrivContextDestroyAM(IntPtr context);

        [DllImport(vg)]
        extern static IntPtr vgPrivSurfaceCreateAM(int width, int height,uint linearColorSpace, uint alphaPremultiplied, uint alphaMask);

        [DllImport(vg)]
        extern static void vgPrivSurfaceDestroyAM(IntPtr surface);

        [DllImport(vg)]
        extern static uint vgPrivMakeCurrentAM(IntPtr context, IntPtr surface);


        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }

        public void SwapBuffers()
        {
            Glfw.SwapBuffers(window);
            Glfw.PollEvents();
        }

        #region VG

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

        [DllImport(vg, EntryPoint = "vgCreatePath")]
        extern static uint vgCreatePath(int pathFormat,
                                PathDatatype datatype,
                                float scale, float bias,
                                int segmentCapacityHint,
                                int coordCapacityHint,
                                PathCapabilities capabilities);
        public uint CreatePath(
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

        [DllImport(vg, EntryPoint = "vgDestroyPath")]
        extern static void vgDestroyPath(uint path);
        public void DestroyPath(uint path)
        {
            vgDestroyPath(path);
        }

        [DllImport(vg, EntryPoint = "vgDrawPath")]
        extern static void vgDrawPath(uint path, PaintMode paintModes);
        public void DrawPath(uint path, PaintMode paintModes)
        {
            vgDrawPath(path, paintModes);
        }

        [DllImport(vg, EntryPoint = "vgCreatePaint")]
        extern static uint vgCreatePaint();
        public uint CreatePaint()
        {
            return vgCreatePaint();
        }

        [DllImport(vg, EntryPoint = "vgDestroyPaint")]
        extern static void vgDestroyPaint(uint paint);
        public void DestroyPaint(uint paint)
        {
            vgDestroyPaint(paint);
        }

        [DllImport(vg, EntryPoint = "vgSetPaint")]
        extern static void vgSetPaint(uint paint, PaintMode paintModes);
        public void SetPaint(uint paint, PaintMode paintModes)
        {
            vgSetPaint(paint, paintModes);
        }

        #endregion

        #region VGU

        [DllImport(vg, EntryPoint = "vguLine")]
        extern public static uint vgLine(uint path, float x0, float y0, float x1, float y1);
        public uint Line(uint path, float x0, float y0, float x1, float y1)
        {
            return vgLine(path, x0, y0, x1, y1);
        }

        [DllImport(vg, EntryPoint = "vguRoundRect")]
        extern public static uint vgRoundRect(uint path, float x, float y, float width, float height, float arcWidth, float arcHeight);
        public uint RoundRect(uint path, float x, float y, float width, float height, float arcWidth, float arcHeight)
        {
            return vgRoundRect(path, x, y, width, height, arcWidth, arcHeight);
        }

        #endregion
    }
}