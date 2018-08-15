using System;

namespace e_sharp_minor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Startup");
            using (var bcmDisplay = new VC.BcmDisplay(0))
            {
                Console.WriteLine("Display = {0}x{1}", bcmDisplay.width, bcmDisplay.height);
                using (var dispmanxDisplay = bcmDisplay.CreateDispmanXDisplay())
                {
                    Console.WriteLine("Display initialized");
                    using (var eglContext = dispmanxDisplay.CreateEGLContext())
                    {
                        Console.WriteLine("EGL initialized");
                    }
                }
            }
            Console.WriteLine("Shutdown");
        }
    }
}
