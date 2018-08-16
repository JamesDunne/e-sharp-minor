using System;
using System.Threading;

namespace e_sharp_minor
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bcmDisplay = new VC.BcmDisplay(0))
            using (var dispmanxDisplay = bcmDisplay.CreateDispmanXDisplay())
            using (var eglContext = dispmanxDisplay.CreateEGLContext())
            {
                Console.WriteLine("Display[0] = {0}x{1}", bcmDisplay.width, bcmDisplay.height);
                eglContext.SwapBuffers();
                Thread.Sleep(1000);
            }
        }
    }
}
