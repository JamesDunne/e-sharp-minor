using System;

namespace e_sharp_minor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Startup");
            Bcm.Host.Init();
            Console.WriteLine("Hello World!");
            uint width, height;
            Bcm.Host.GetDisplaySize(0, out width, out height);
            Console.WriteLine("Display = {0}x{1}", width, height);
            Bcm.Host.Deinit();
            Console.WriteLine("Shutdown");
        }
    }
}
