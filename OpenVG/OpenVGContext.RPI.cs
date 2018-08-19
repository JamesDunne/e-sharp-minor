using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

// Don't listen to any "field not assigned" warnings.
#pragma warning disable CS0649

namespace OpenVG
{
    public partial class OpenVGContext : IDisposable
    {
#if RPI
        internal readonly ushort bcmDisplay;

        internal uint dispman_display;
        internal uint dispman_update;
        internal uint dispman_element;

        public readonly int majorVersion, minorVersion;

        internal readonly uint egldisplay;
        internal readonly uint eglsurface;
        internal readonly uint eglcontext;

        internal readonly EGL_DISPMANX_WINDOW_T window;

        public OpenVGContext(int display)
        {
            bcmDisplay = (ushort)display;

            bcm_host_init();

            uint bcmWidth, bcmHeight;
            graphics_get_display_size(bcmDisplay, out bcmWidth, out bcmHeight);

            Width = (int)bcmWidth;
            Height = (int)bcmHeight;

            VC_RECT_T dst_rect;
            VC_RECT_T src_rect;

            dst_rect.x = 0;
            dst_rect.y = 0;
            dst_rect.width = Width;
            dst_rect.height = Height;

            src_rect.x = 0;
            src_rect.y = 0;
            src_rect.width = Width << 16;
            src_rect.height = Height << 16;

            // TODO: translate error codes into exceptions?
            Debug.WriteLine("vc_dispmanx_display_open(...)");
            dispman_display = vc_dispmanx_display_open(bcmDisplay);
            Debug.WriteLine("dispman_display = {0}", dispman_display);
            Debug.WriteLine("vc_dispmanx_update_start(...)");
            dispman_update = vc_dispmanx_update_start(0 /* priority */);
            Debug.WriteLine("dispman_update = {0}", dispman_update);

            Debug.WriteLine("vc_dispmanx_element_add(...)");
            dispman_element = vc_dispmanx_element_add(
                dispman_update,
                dispman_display,
                1 /*layer*/,
                ref dst_rect,
                0 /*src*/,
                ref src_rect,
                DISPMANX_PROTECTION_T.DISPMANX_PROTECTION_NONE,
                IntPtr.Zero /*alpha*/,
                IntPtr.Zero /*clamp*/,
                0 /*transform*/
            );
            Debug.WriteLine("dispman_element = {0}", dispman_element);

            Debug.WriteLine("vc_dispmanx_update_submit_sync(dispman_update)");
            checkError(vc_dispmanx_update_submit_sync(dispman_update));



            // TODO: validate this assumption that display number goes here (what other values to use besides EGL_DEFAULT_DISPLAY?)
            Debug.WriteLine("eglGetDisplay(...)");
            egldisplay = eglGetDisplay(bcmDisplay);
            //this.egldisplay = eglGetDisplay(EGL_DEFAULT_DISPLAY);
            throwIfError();
            Debug.WriteLine("egldisplay = {0}", egldisplay);

            Debug.WriteLine("eglInitialize(egldisplay)");
            eglInitialize(egldisplay, out majorVersion, out minorVersion);
            throwIfError();
            Debug.WriteLine("eglBindAPI(EGL_OPENVG_API)");
            eglBindAPI(EGL.EGL_OPENVG_API);
            throwIfError();

            int[] attribs = new int[] {
                (int)EGL_ATTRIBUTES.EGL_COLOR_BUFFER_TYPE,  (int)EGL.EGL_RGB_BUFFER,
                (int)EGL_ATTRIBUTES.EGL_SURFACE_TYPE,       (int)EGL_ATTRIBUTES.EGL_WINDOW_BIT,
                (int)EGL_ATTRIBUTES.EGL_SAMPLES,            1,
                (int)EGL_ATTRIBUTES.EGL_RED_SIZE,           8,
                (int)EGL_ATTRIBUTES.EGL_GREEN_SIZE,         8,
                (int)EGL_ATTRIBUTES.EGL_BLUE_SIZE,          8,
                (int)EGL_ATTRIBUTES.EGL_ALPHA_SIZE,         8,
                (int)EGL_ATTRIBUTES.EGL_LUMINANCE_SIZE,     0,
                (int)EGL_ATTRIBUTES.EGL_NONE
            };
            int numconfigs;
            uint eglconfig;

            Debug.WriteLine("eglChooseConfig(egldisplay, ...)");
            eglChooseConfig(egldisplay, attribs, out eglconfig, 1, out numconfigs);
            throwIfError();
            if (numconfigs != 1)
            {
                throw new Exception("numconfigs != 1");
            }
            Debug.WriteLine("eglconfig = {0}", eglconfig);

            window.element = dispman_element;
            window.width = Width;
            window.height = Height;

            Debug.WriteLine("eglCreateWindowSurface(egldisplay, ...)");
            eglsurface = eglCreateWindowSurface(egldisplay, eglconfig, ref window, null);
            throwIfError();
            Debug.WriteLine("eglsurface = {0}", eglsurface);
            Debug.WriteLine("eglCreateContext(egldisplay, ...)");
            eglcontext = eglCreateContext(egldisplay, eglconfig, 0, null);
            throwIfError();
            Debug.WriteLine("eglcontext = {0}", eglcontext);
            Debug.WriteLine("eglMakeCurrent(egldisplay, eglsurface, eglsurface, eglcontext)");
            eglMakeCurrent(egldisplay, eglsurface, eglsurface, eglcontext);
            throwIfError();

            // Translate to pixel-perfect offset:
            vgSeti(OpenVG.ParamType.VG_MATRIX_MODE, (int)OpenVG.MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
            vgLoadIdentity();
            vgTranslate(0.5f, 0.5f);
        }

        public void Dispose()
        {
            Debug.WriteLine("eglMakeCurrent(display, EGL_NO_SURFACE, EGL_NO_SURFACE, EGL_NO_CONTEXT)");
            eglMakeCurrent(egldisplay, (uint)EGL.EGL_NO_SURFACE, (uint)EGL.EGL_NO_SURFACE, (uint)EGL.EGL_NO_CONTEXT);
            throwIfError();

            Debug.WriteLine("eglTerminate(display)");
            eglTerminate(egldisplay);
            throwIfError();

            if ((majorVersion > 1) || (majorVersion == 1 && minorVersion >= 2))
            {
                Debug.WriteLine("eglReleaseThread(display)");
                eglReleaseThread();
                throwIfError();
            }


            Debug.WriteLine("vc_dispmanx_element_remove(update, element)");
            checkError(vc_dispmanx_element_remove(dispman_update, dispman_element));
            Debug.WriteLine("vc_dispmanx_display_close(display)");
            checkError(vc_dispmanx_display_close(dispman_display));


            bcm_host_deinit();
        }

        private void throwIfError()
        {
            EGL_ERROR err = eglGetError();
            if (err != EGL_ERROR.EGL_SUCCESS)
            {
                throw new Exception(String.Format("EGL error code {0:4X}", err));
            }
        }

        private void checkError(int ret)
        {
            if (ret != 0)
            {
                throw new Exception(String.Format("dispmanx function returned {0}", ret));
            }
        }

        public void SwapBuffers()
        {
            Debug.WriteLine("eglSwapBuffers(display, surface)");
            eglSwapBuffers(egldisplay, eglsurface);
        }

        #region DispmanX

        [DllImport("bcm_host", EntryPoint = "bcm_host_init")]
        internal extern static void bcm_host_init();
        [DllImport("bcm_host", EntryPoint = "bcm_host_deinit")]
        internal extern static void bcm_host_deinit();

        [DllImport("bcm_host", EntryPoint = "graphics_get_display_size")]
        internal extern static int graphics_get_display_size(ushort displayNumber, out uint width, out uint height);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_display_open")]
        extern static uint vc_dispmanx_display_open(uint device);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_update_start")]
        extern static uint vc_dispmanx_update_start(int priority);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_element_add")]
        extern static uint vc_dispmanx_element_add(
            uint update,
            uint display,
            int layer,
            ref VC_RECT_T dest_rect,
            uint src,
            ref VC_RECT_T src_rect,
            DISPMANX_PROTECTION_T protection,
            IntPtr /* ref VC_DISPMANX_ALPHA_T */ alpha,
            IntPtr /* ref DISPMANX_CLAMP_T */ clamp,
            DISPMANX_TRANSFORM_T transform
        );

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_element_remove")]
        extern static int vc_dispmanx_element_remove(uint update, uint element);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_update_submit_sync")]
        extern static int vc_dispmanx_update_submit_sync(uint update);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_display_close")]
        extern static int vc_dispmanx_display_close(uint display);

        #endregion

        #region EGL

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

        [DllImport(eglName, EntryPoint = "eglDestroySurface")]
        extern static uint eglDestroySurface(uint dpy, uint surface); // returns EGLBoolean

        [DllImport(eglName, EntryPoint = "eglCreateContext")]
        extern static uint eglCreateContext(
            uint dpy,
            uint config,
            uint share_context,
            int[] attrib_list
        ); // returns EGLContext

        [DllImport(eglName, EntryPoint = "eglDestroyContext")]
        extern static uint eglDestroyContext(uint dpy, uint ctx); // returns EGLBoolean

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
#endif
    }

#if RPI
    [StructLayout(LayoutKind.Sequential)]
    internal struct VC_RECT_T
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }

    internal enum DISPMANX_PROTECTION_T : uint
    {
        DISPMANX_PROTECTION_NONE = 0,
        DISPMANX_PROTECTION_HDCP = 11,   // Derived from the WM DRM levels, 101-300
        DISPMANX_PROTECTION_MAX = 0x0f
    }

    internal enum DISPMANX_TRANSFORM_T : uint
    {
        /* Bottom 2 bits sets the orientation */
        DISPMANX_NO_ROTATE = 0,
        DISPMANX_ROTATE_90 = 1,
        DISPMANX_ROTATE_180 = 2,
        DISPMANX_ROTATE_270 = 3,

        DISPMANX_FLIP_HRIZ = 1 << 16,
        DISPMANX_FLIP_VERT = 1 << 17,

        /* invert left/right images */
        DISPMANX_STEREOSCOPIC_INVERT = 1 << 19,
        /* extra flags for controlling 3d duplication behaviour */
        DISPMANX_STEREOSCOPIC_NONE = 0 << 20,
        DISPMANX_STEREOSCOPIC_MONO = 1 << 20,
        DISPMANX_STEREOSCOPIC_SBS = 2 << 20,
        DISPMANX_STEREOSCOPIC_TB = 3 << 20,
        DISPMANX_STEREOSCOPIC_MASK = 15 << 20,

        /* extra flags for controlling snapshot behaviour */
        DISPMANX_SNAPSHOT_NO_YUV = 1 << 24,
        DISPMANX_SNAPSHOT_NO_RGB = 1 << 25,
        DISPMANX_SNAPSHOT_FILL = 1 << 26,
        DISPMANX_SNAPSHOT_SWAP_RED_BLUE = 1 << 27,
        DISPMANX_SNAPSHOT_PACK = 1 << 28
    }

    internal enum DISPMANX_FLAGS_ALPHA_T
    {
        /* Bottom 2 bits sets the alpha mode */
        DISPMANX_FLAGS_ALPHA_FROM_SOURCE = 0,
        DISPMANX_FLAGS_ALPHA_FIXED_ALL_PIXELS = 1,
        DISPMANX_FLAGS_ALPHA_FIXED_NON_ZERO = 2,
        DISPMANX_FLAGS_ALPHA_FIXED_EXCEED_0X07 = 3,

        DISPMANX_FLAGS_ALPHA_PREMULT = 1 << 16,
        DISPMANX_FLAGS_ALPHA_MIX = 1 << 17
    }

    internal struct VC_DISPMANX_ALPHA_T
    {
        public DISPMANX_FLAGS_ALPHA_T flags;
        public uint opacity;
        public uint mask;
    }

    enum DISPMANX_FLAGS_CLAMP_T : uint
    {
        DISPMANX_FLAGS_CLAMP_NONE = 0,
        DISPMANX_FLAGS_CLAMP_LUMA_TRANSPARENT = 1,
        // NOTE(jsd): Wild guess here.
        //#if __VCCOREVER__ >= 0x04000000
        DISPMANX_FLAGS_CLAMP_TRANSPARENT = 2,
        DISPMANX_FLAGS_CLAMP_REPLACE = 3
        //#else
        //        DISPMANX_FLAGS_CLAMP_CHROMA_TRANSPARENT = 2,
        //        DISPMANX_FLAGS_CLAMP_TRANSPARENT = 3
        //#endif
    }

    enum DISPMANX_FLAGS_KEYMASK_T : uint
    {
        DISPMANX_FLAGS_KEYMASK_OVERRIDE = 1,
        DISPMANX_FLAGS_KEYMASK_SMOOTH = 1 << 1,
        DISPMANX_FLAGS_KEYMASK_CR_INV = 1 << 2,
        DISPMANX_FLAGS_KEYMASK_CB_INV = 1 << 3,
        DISPMANX_FLAGS_KEYMASK_YY_INV = 1 << 4
    }

    internal struct DISPMANX_CLAMP_KEYS_T
    {
        public byte red_upper;
        public byte red_lower;
        public byte blue_upper;
        public byte blue_lower;
        public byte green_upper;
        public byte green_lower;
    }

    internal struct DISPMANX_CLAMP_T
    {
        public DISPMANX_FLAGS_CLAMP_T mode;
        public DISPMANX_FLAGS_KEYMASK_T key_mask;
        public DISPMANX_CLAMP_KEYS_T key_value;
        public uint replace_value;
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

        EGL_RGB_BUFFER = 0x308E,    /* EGL_COLOR_BUFFER_TYPE value */
        EGL_LUMINANCE_BUFFER = 0x308F,	/* EGL_COLOR_BUFFER_TYPE value */

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
        EGL_COLOR_BUFFER_TYPE = 0x303F,

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
        EGL_SUCCESS = 0x3000,
        EGL_NOT_INITIALIZED = 0x3001,
        EGL_BAD_ACCESS = 0x3002,
        EGL_BAD_ALLOC = 0x3003,
        EGL_BAD_ATTRIBUTE = 0x3004,
        EGL_BAD_CONFIG = 0x3005,
        EGL_BAD_CONTEXT = 0x3006,
        EGL_BAD_CURRENT_SURFACE = 0x3007,
        EGL_BAD_DISPLAY = 0x3008,
        EGL_BAD_MATCH = 0x3009,
        EGL_BAD_NATIVE_PIXMAP = 0x300A,
        EGL_BAD_NATIVE_WINDOW = 0x300B,
        EGL_BAD_PARAMETER = 0x300C,
        EGL_BAD_SURFACE = 0x300D,
        EGL_CONTEXT_LOST = 0x300E	/* EGL 1.1 - IMG_power_management */
    }
#endif
}
