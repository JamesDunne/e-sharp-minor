using System;
using System.Threading;

namespace e_sharp_minor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Startup");

            using (var bcmDisplay = new VC.BcmDisplay(0))
            {
                Console.WriteLine("Display[0] = {0}x{1}", bcmDisplay.width, bcmDisplay.height);
                using (var dispmanxDisplay = bcmDisplay.CreateDispmanXDisplay())
                {
                    Console.WriteLine("DispmanX initialized");
                    using (var eglContext = dispmanxDisplay.CreateEGLContext())
                    {
                        Console.WriteLine("EGL initialized");
                        eglContext.SwapBuffers();
                        Thread.Sleep(1000);
                    }
                    Console.WriteLine("EGL destroyed");
                }
                Console.WriteLine("DispmanX destroyed");
            }

            Console.WriteLine("Shutdown");
        }
    }
}
