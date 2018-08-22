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
            var controller = new Controller();
            controller.LoadData();
            //new VGUI(controller).Run();
        }
    }
}
