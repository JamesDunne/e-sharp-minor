using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OpenVG
{
    public partial class OpenVGContext : IDisposable
    {
#if ! RPI
        const string vg = "AmanithVG";

        internal readonly IntPtr vgContext;
        internal readonly IntPtr vgSurface;

        internal readonly Glfw.Window window;

        public OpenVGContext(int width, int height)
        {
            // Window coordinates (non-retina):
            this.Width = width;
            this.Height = height;

            Debug.WriteLine("glfw.Init()");
            Glfw.Init();

            Debug.WriteLine("window = glfw.CreateWindow()");
            window = Glfw.CreateWindow(width, height, "e-sharp-minor");

            Debug.WriteLine("glfw.MakeContextCurrent(window)");
            Glfw.MakeContextCurrent(window);

            // Get the real framebuffer size for OpenGL pixels; should work with Retina:
            int fbWidth, fbHeight;
            Glfw.GetFramebufferSize(window, out fbWidth, out fbHeight);

            // create an OpenVG context
            Debug.WriteLine("vgContext = vgPrivContextCreateAM(0)");
            vgContext = vgPrivContextCreateAM(IntPtr.Zero);

            // create a drawing surface (sRGBA premultiplied color space)
            vgSurface = vgPrivSurfaceCreateAM(fbWidth, fbHeight, 0, 1, 1);

            // bind context and surface
            vgPrivMakeCurrentAM(vgContext, vgSurface);

            // Apply scale for retina display:
            vgSeti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
            vgLoadIdentity();
            vgScale((float)fbWidth / (float)width, (float)fbHeight / (float)height);
            vgTranslate(0.5f, 0.5f);

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

        [DllImport(vg)]
        extern static IntPtr vgPrivContextCreateAM(IntPtr sharedContext);

        [DllImport(vg)]
        extern static void vgPrivContextDestroyAM(IntPtr context);

        [DllImport(vg)]
        extern static IntPtr vgPrivSurfaceCreateAM(int width, int height, uint linearColorSpace, uint alphaPremultiplied, uint alphaMask);

        [DllImport(vg)]
        extern static void vgPrivSurfaceDestroyAM(IntPtr surface);

        [DllImport(vg)]
        extern static uint vgPrivMakeCurrentAM(IntPtr context, IntPtr surface);

        public void SwapBuffers()
        {
            Glfw.SwapBuffers(window);
            Glfw.PollEvents();

            double cx, cy;
            Glfw.GetCursorPos(window, out cx, out cy);
            Console.WriteLine("{0},{1}", cx, cy);
        }
#endif

        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }
    }
}