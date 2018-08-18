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
            this.Width = width;
            this.Height = height;

            Debug.WriteLine("glfw.Init()");
            Glfw.Init();

            Debug.WriteLine("window = glfw.CreateWindow()");
            window = Glfw.CreateWindow(width, height, "e-sharp-minor");

            Debug.WriteLine("glfw.MakeContextCurrent(window)");
            Glfw.MakeContextCurrent(window);

            // create an OpenVG context
            Debug.WriteLine("vgContext = vgPrivContextCreateAM(0)");
            vgContext = vgPrivContextCreateAM(IntPtr.Zero);

#if false
            // create a drawing surface (sRGBA premultiplied color space)
            vgSurface = vgPrivSurfaceCreateMZT(width, height, VG_FALSE, VG_TRUE, VG_TRUE);

            // bind context and surface
            vgPrivMakeCurrentMZT(vgContext, vgSurface);
#endif

            Debug.WriteLine("glfw.ShowWindow(window)");
            Glfw.ShowWindow(window);
        }

        public void Dispose()
        {
            Glfw.HideWindow(window);

            // TODO: destroy OpenVG context.

            Debug.WriteLine("vgPrivContextDestroyAM(vgContext)");
            vgPrivContextDestroyAM(vgContext);

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

        public int Geti(ParamType type)
        {
            return 0;
        }

        public float Getf(ParamType type)
        {
            return 0f;
        }

        public float[] Getfv(ParamType type)
        {
            return null;
        }

        public int[] Getiv(ParamType type)
        {
            return null;
        }

        public void Seti(ParamType paramType, int value)
        {

        }

        public void Setf(ParamType paramType, float value)
        {

        }

        public void Setfv(ParamType paramType, float[] values)
        {

        }

        public void SetParameteri(uint handle, int paramType, int value)
        {

        }

        public void SetParameterfv(uint handle, int paramType, float[] values)
        {

        }

        public void Clear(int x, int y, int width, int height)
        {

        }

        public uint CreatePath(
            int pathFormat,
            PathDatatype datatype,
            float scale, float bias,
            int segmentCapacityHint,
            int coordCapacityHint,
            PathCapabilities capabilities
        )
        {
            return 0;
        }

        public void DestroyPath(uint path)
        {

        }

        public void DrawPath(uint path, PaintMode paintModes)
        {

        }

        public uint CreatePaint()
        {
            return 0;
        }

        public void DestroyPaint(uint paint)
        {

        }

        public void SetPaint(uint paint, PaintMode paintModes)
        {

        }

        #endregion

        #region VGU

        public uint Line(uint path, float x0, float y0, float x1, float y1)
        {
            return 0;
        }

        public uint RoundRect(uint path, float x, float y, float width, float height, float arcWidth, float arcHeight)
        {
            return 0;
        }

        #endregion
    }
}