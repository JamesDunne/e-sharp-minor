using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenVG;

namespace e_sharp_minor
{
    public class GlfwPlatform : IPlatform
    {
        private readonly MidiConsoleOut midi;
        private readonly OpenVGContext vg;

        internal readonly IntPtr vgContext;
        internal readonly IntPtr vgSurface;

        internal readonly Glfw.Window window;

        public GlfwPlatform(int width, int height)
        {
            midi = new MidiConsoleOut();

            // Window coordinates (non-retina):
            this.Width = width;
            this.Height = height;

            // Handle Glfw errors:
            Glfw.SetErrorCallback((code, desc) => throw new Exception(String.Format("GLFW error code {0}: {1}", code, desc)));

            Debug.WriteLine("glfw.Init()");
            Glfw.Init();

            Debug.WriteLine("window = glfw.CreateWindow()");
            window = Glfw.CreateWindow(width, height, "e-sharp-minor");

            Debug.WriteLine("glfw.MakeContextCurrent(window)");
            Glfw.MakeContextCurrent(window);

            // Get the real framebuffer size for OpenGL pixels; should work with Retina:
            int fbWidth, fbHeight;
            Glfw.GetFramebufferSize(window, out fbWidth, out fbHeight);

            // These are needed for vgClear since it works in framebuffer pixels.
            FramebufferWidth = fbWidth;
            FramebufferHeight = fbHeight;

            // create an OpenVG context
            Debug.WriteLine("vgContext = vgPrivContextCreateAM(0)");
            vgContext = vgPrivContextCreateAM(IntPtr.Zero);

            // create a drawing surface (sRGBA premultiplied color space)
            vgSurface = vgPrivSurfaceCreateAM(fbWidth, fbHeight, 0, 1, 1);

            // bind context and surface
            vgPrivMakeCurrentAM(vgContext, vgSurface);

            // Create OpenVGContext:
            vg = new OpenVGContext();

            // Apply scale for retina display:
            vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
            vg.LoadIdentity();
            vg.Scale((float)fbWidth / (float)width, (float)fbHeight / (float)height);
            vg.Translate(0.5f, 0.5f);

            Debug.WriteLine("glfw.ShowWindow(window)");
            Glfw.ShowWindow(window);
        }

        public IOpenVG VG => vg;

        public IMIDI MIDI => midi;

        public event InputEventDelegate InputEvent;

        public int Width { get; }
        public int Height { get; }

        public int FramebufferWidth { get; }
        public int FramebufferHeight { get; }

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

        /// <summary>
        /// Determines if GUI user indicated application should quit.
        /// </summary>
        /// <returns><c>true</c>, if should quit, <c>false</c> otherwise.</returns>
        public bool ShouldQuit()
        {
            return Glfw.WindowShouldClose(window);
        }

        #region AmanithVG

        const string vgLib = "OpenVG"; // really AmanithVG but renamed

        [DllImport(vgLib)]
        extern static IntPtr vgPrivContextCreateAM(IntPtr sharedContext);

        [DllImport(vgLib)]
        extern static void vgPrivContextDestroyAM(IntPtr context);

        [DllImport(vgLib)]
        extern static IntPtr vgPrivSurfaceCreateAM(int width, int height, uint linearColorSpace, uint alphaPremultiplied, uint alphaMask);

        [DllImport(vgLib)]
        extern static void vgPrivSurfaceDestroyAM(IntPtr surface);

        [DllImport(vgLib)]
        extern static uint vgPrivMakeCurrentAM(IntPtr context, IntPtr surface);

        #endregion

        public void SwapBuffers()
        {
            Glfw.SwapBuffers(window);
            // TODO: is this necessary to handle window events?
            Glfw.PollEvents();

            //double cx, cy;
            //Glfw.GetCursorPos(window, out cx, out cy);
            //Console.WriteLine("{0},{1}", cx, cy);
        }

    }
}
