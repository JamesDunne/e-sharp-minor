using System;
using System.Threading;
using VC;

namespace e_sharp_minor
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bcmDisplay = new BcmDisplay(0))
            using (var dispmanxDisplay = bcmDisplay.CreateDispmanXDisplay())
            using (var eglContext = dispmanxDisplay.CreateEGLContext())
            {
                Console.WriteLine("Display[0] = {0}x{1}", bcmDisplay.width, bcmDisplay.height);
                VG.Setfv(VG.ParamType.VG_CLEAR_COLOR, new float[] { 0, 0, 0, 1.0f });
                VG.Clear(0, 0, (int)bcmDisplay.width, (int)bcmDisplay.height);
                eglContext.SwapBuffers();

                Thread.Sleep(1000);
            }
        }
    }
}
