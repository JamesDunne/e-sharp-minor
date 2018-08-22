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
#if false
            var translator = new Translator();
            translator.Translate();
#else
            IMIDI midi;
            midi = new MidiConsoleOut();
            midi = new MidiState(midi);

            var controller = new Controller(midi, 2);

            controller.LoadData();

            //new VGUI(controller).Run();
#endif
        }
    }
}
