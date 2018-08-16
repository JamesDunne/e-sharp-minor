using System;
using System.Runtime.InteropServices;

namespace VC
{
    public class EGLContext : IDisposable
    {
        private readonly DispmanXDisplay dispmanXDisplay;

        internal readonly uint egldisplay;
        internal readonly uint eglsurface;
        internal readonly uint eglcontext;

        internal EGLContext(DispmanXDisplay dispmanXDisplay)
        {
            this.dispmanXDisplay = dispmanXDisplay;

            int[] s_configAttribs = new int[]{
                (int)EGL_ATTRIBUTES.EGL_RED_SIZE,       8,
                (int)EGL_ATTRIBUTES.EGL_GREEN_SIZE,     8,
                (int)EGL_ATTRIBUTES.EGL_BLUE_SIZE,      8,
                (int)EGL_ATTRIBUTES.EGL_ALPHA_SIZE,     8,
                (int)EGL_ATTRIBUTES.EGL_LUMINANCE_SIZE, (int)EGL_ATTRIBUTES.EGL_DONT_CARE,        //EGL_DONT_CARE
                (int)EGL_ATTRIBUTES.EGL_SURFACE_TYPE,   (int)EGL_ATTRIBUTES.EGL_WINDOW_BIT,
                (int)EGL_ATTRIBUTES.EGL_SAMPLES,        1,
                (int)EGL_ATTRIBUTES.EGL_NONE
            };
            int numconfigs;
            uint eglconfig;

            // TODO: validate this assumption that display number goes here (what other values to use besides EGL_DEFAULT_DISPLAY?)
            this.egldisplay = eglGetDisplay(this.dispmanXDisplay.bcmDisplay.display);
            //this.egldisplay = eglGetDisplay(EGL_DEFAULT_DISPLAY);

            int major, minor;
            eglInitialize(egldisplay, out major, out minor);
            throwIfError();
            eglBindAPI(EGL.EGL_OPENVG_API);

            eglChooseConfig(egldisplay, s_configAttribs, out eglconfig, 1, out numconfigs);
            throwIfError();
            // assert(numconfigs == 1);

            EGL_DISPMANX_WINDOW_T window;
            window.element = this.dispmanXDisplay.dispman_element;
            window.width = (int)this.dispmanXDisplay.bcmDisplay.width;
            window.height = (int)this.dispmanXDisplay.bcmDisplay.height;

            eglsurface = eglCreateWindowSurface(egldisplay, eglconfig, ref window, null);
            throwIfError();
            eglcontext = eglCreateContext(egldisplay, eglconfig, 0, null);
            throwIfError();
            eglMakeCurrent(egldisplay, eglsurface, eglsurface, eglcontext);
            throwIfError();
        }

        public void Dispose()
        {
            eglMakeCurrent(egldisplay, (uint)EGL.EGL_NO_SURFACE, (uint)EGL.EGL_NO_SURFACE, (uint)EGL.EGL_NO_CONTEXT);
            throwIfError();
            eglTerminate(egldisplay);
            throwIfError();
            eglReleaseThread();
            throwIfError();
        }

        private void throwIfError()
        {
            EGL_ERROR err = eglGetError();
            if (err != EGL_ERROR.EGL_SUCCESS)
            {
                throw new Exception(String.Format("EGL error code {0:4X}", err));
            }
        }

        public void SwapBuffers()
        {
            eglSwapBuffers(egldisplay, eglsurface);
        }

        #region DllImports

        // This should be "EGL" but libEGL.so on raspbian is missing some symbols that are found in libGLESv2.so
        const string eglName = "GLESv2";

        [DllImport(eglName, EntryPoint = "eglGetError")]
        extern static EGL_ERROR eglGetError();

        [DllImport(eglName, EntryPoint = "eglGetDisplay")]
        extern static uint eglGetDisplay(uint display);

        [DllImport(eglName, EntryPoint = "eglInitialize")]
        extern static uint eglInitialize(uint dpy, out int major, out int minor); // returns EGLboolean

        [DllImport(eglName, EntryPoint = "eglBindAPI")]
        extern static uint eglBindAPI(EGL api); // returns EGLboolean

        [DllImport(eglName, EntryPoint = "eglChooseConfig")]
        extern static uint eglChooseConfig(
            uint dpy,
            int[] attrib_list,
            out uint configs,
            int config_size,
            out int num_config
        ); // returns EGLBoolean

        [DllImport(eglName, EntryPoint = "eglCreateWindowSurface")]
        extern static uint eglCreateWindowSurface(
            uint dpy,
            uint config,
            ref EGL_DISPMANX_WINDOW_T win,
            int[] attrib_list
        ); // returns EGLSurface

        [DllImport(eglName, EntryPoint = "eglCreateContext")]
        extern static uint eglCreateContext(
            uint dpy,
            uint config,
            uint share_context,
            int[] attrib_list
        ); // returns EGLContext

        [DllImport(eglName, EntryPoint = "eglMakeCurrent")]
        extern static uint eglMakeCurrent(
           uint dpy,
           uint draw,
           uint read,
           uint ctx
        ); // returns EGLBoolean

        [DllImport(eglName, EntryPoint = "eglTerminate")]
        extern static uint eglTerminate(uint dpy); // returns EGLBoolean

        [DllImport(eglName, EntryPoint = "eglReleaseThread")]
        extern static uint eglReleaseThread(); // returns EGLBoolean

        [DllImport(eglName, EntryPoint = "eglSwapBuffers")]
        extern static uint eglSwapBuffers(uint dpy, uint surface); // returns EGLBoolean

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct EGL_DISPMANX_WINDOW_T
    {
        public uint element;
        public int width;   /* This is necessary because dispmanx elements are not queriable. */
        public int height;
    }

    internal enum EGL : uint
    {
        EGL_NO_SURFACE = 0,
        EGL_NO_CONTEXT = 0,
        EGL_NO_DISPLAY = 0,

        EGL_OPENGL_ES_API = 0x30A0,
        EGL_OPENVG_API = 0x30A1,
        EGL_OPENGL_API = 0x30A2
    }

    internal enum EGL_ATTRIBUTES : int
    {
        EGL_DONT_CARE = -1,

        /* Config attributes */
        EGL_ALPHA_SIZE = 0x3021,
        EGL_BLUE_SIZE = 0x3022,
        EGL_GREEN_SIZE = 0x3023,
        EGL_RED_SIZE = 0x3024,
        EGL_SAMPLES = 0x3031,
        EGL_SURFACE_TYPE = 0x3033,
        EGL_NONE = 0x3038,   /* Attrib list terminator */
        EGL_LUMINANCE_SIZE = 0x303D,

        /* Config attribute mask bits */
        EGL_PBUFFER_BIT = 0x0001,   /* EGL_SURFACE_TYPE mask bits */
        EGL_PIXMAP_BIT = 0x0002,   /* EGL_SURFACE_TYPE mask bits */
        EGL_WINDOW_BIT = 0x0004,   /* EGL_SURFACE_TYPE mask bits */
        EGL_VG_COLORSPACE_LINEAR_BIT = 0x0020,   /* EGL_SURFACE_TYPE mask bits */
        EGL_VG_ALPHA_FORMAT_PRE_BIT = 0x0040,   /* EGL_SURFACE_TYPE mask bits */
        EGL_MULTISAMPLE_RESOLVE_BOX_BIT = 0x0200,  /* EGL_SURFACE_TYPE mask bits */
        EGL_SWAP_BEHAVIOR_PRESERVED_BIT = 0x0400  /* EGL_SURFACE_TYPE mask bits */
    }

    internal enum EGL_ERROR : uint
    {
        EGL_SUCCESS =			0x3000,
        EGL_NOT_INITIALIZED =		0x3001,
        EGL_BAD_ACCESS =			0x3002,
        EGL_BAD_ALLOC =			0x3003,
        EGL_BAD_ATTRIBUTE =		0x3004,
        EGL_BAD_CONFIG =			0x3005,
        EGL_BAD_CONTEXT =			0x3006,
        EGL_BAD_CURRENT_SURFACE =		0x3007,
        EGL_BAD_DISPLAY =			0x3008,
        EGL_BAD_MATCH =			0x3009,
        EGL_BAD_NATIVE_PIXMAP =		0x300A,
        EGL_BAD_NATIVE_WINDOW =		0x300B,
        EGL_BAD_PARAMETER =		0x300C,
        EGL_BAD_SURFACE =			0x300D,
        EGL_CONTEXT_LOST =		0x300E	/* EGL 1.1 - IMG_power_management */
    }
}
