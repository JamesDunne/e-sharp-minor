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
            // assert(eglGetError() == EGL_SUCCESS);
            eglBindAPI(EGL.EGL_OPENVG_API);

            eglChooseConfig(egldisplay, s_configAttribs, out eglconfig, 1, out numconfigs);
            // assert(eglGetError() == EGL_SUCCESS);
            // assert(numconfigs == 1);

            EGL_DISPMANX_WINDOW_T window;
            window.element = this.dispmanXDisplay.dispman_element;
            window.width = (int)this.dispmanXDisplay.bcmDisplay.width;
            window.height = (int)this.dispmanXDisplay.bcmDisplay.height;

            eglsurface = eglCreateWindowSurface(egldisplay, eglconfig, ref window, null);
            // assert(eglGetError() == EGL_SUCCESS);
            eglcontext = eglCreateContext(egldisplay, eglconfig, 0, null);
            // assert(eglGetError() == EGL_SUCCESS);
            eglMakeCurrent(egldisplay, eglsurface, eglsurface, eglcontext);
            // assert(eglGetError() == EGL_SUCCESS);
        }

        public void Dispose()
        {
            eglMakeCurrent(egldisplay, (uint)EGL.EGL_NO_SURFACE, (uint)EGL.EGL_NO_SURFACE, (uint)EGL.EGL_NO_CONTEXT);
            // assert(eglGetError() == EGL_SUCCESS);
            eglTerminate(egldisplay);
            // assert(eglGetError() == EGL_SUCCESS);
            eglReleaseThread();
        }

        // This should be "EGL" technically, but libEGL.so on raspbian depends on libGLESv2.so or vice-versa, so we import that instead:
        const string eglName = "GLESv2";

        // DllImports:
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
}