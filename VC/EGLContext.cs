using System;
using System.Runtime.InteropServices;

namespace VC
{
    public class EGLContext : IDisposable
    {
        private readonly DispmanXDisplay dispmanXDisplay;

        internal EGLContext(DispmanXDisplay dispmanXDisplay)
        {
            this.dispmanXDisplay = dispmanXDisplay;

            // window.element = dispman_element;
            // window.width = width;
            // window.height = height;

            // static const EGLint s_configAttribs[] =
            // {
            //     EGL_RED_SIZE,       8,
            //     EGL_GREEN_SIZE,     8,
            //     EGL_BLUE_SIZE,      8,
            //     EGL_ALPHA_SIZE,     8,
            //     EGL_LUMINANCE_SIZE, EGL_DONT_CARE,        //EGL_DONT_CARE
            //     EGL_SURFACE_TYPE,   EGL_WINDOW_BIT,
            //     EGL_SAMPLES,        1,
            //     EGL_NONE
            // };
            // EGLint numconfigs;

            // egldisplay = eglGetDisplay(EGL_DEFAULT_DISPLAY);
            // eglInitialize(egldisplay, NULL, NULL);
            // assert(eglGetError() == EGL_SUCCESS);
            // eglBindAPI(EGL_OPENVG_API);

            // eglChooseConfig(egldisplay, s_configAttribs, &eglconfig, 1, &numconfigs);
            // assert(eglGetError() == EGL_SUCCESS);
            // assert(numconfigs == 1);

            // eglsurface = eglCreateWindowSurface(egldisplay, eglconfig, window, NULL);
            // assert(eglGetError() == EGL_SUCCESS);
            // eglcontext = eglCreateContext(egldisplay, eglconfig, NULL, NULL);
            // assert(eglGetError() == EGL_SUCCESS);
            // eglMakeCurrent(egldisplay, eglsurface, eglsurface, eglcontext);
            // assert(eglGetError() == EGL_SUCCESS);

        }

        public void Dispose()
        {
            // TODO.
        }
    }
}