using System;

namespace e_sharp_minor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Startup");
            VC.BcmHost.Init();
            Console.WriteLine("Hello World!");
            uint width, height;
            VC.BcmHost.GetDisplaySize(0, out width, out height);
            Console.WriteLine("Display = {0}x{1}", width, height);
            VC.BcmHost.Deinit();
            Console.WriteLine("Shutdown");
        }
    }
}
