using System;
using System.Diagnostics;
using System.Threading;
using OpenVG;
using Shapes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace e_sharp_minor
{
    class MainClass
    {
        static void Main(string[] args)
        {

#if RPI
            using (var vg = new OpenVGContext(0))
#else
            using (var vg = new OpenVGContext(800, 480))
#endif
            {
                Console.WriteLine("Display[0] = {0}x{1}", vg.Width, vg.Height);

                var controller = new Controller(vg);
                controller.Run();
            }
        }
    }
}
