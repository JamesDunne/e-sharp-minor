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

            // Initialize MIDI OUT device:
#if RPI
            using (IMIDI midi = new MidiAlsaOut())
#else
            using (IMIDI midi = new MidiConsoleOut())
#endif
            {
                var controller = new Controller(midi, 2);
                controller.LoadData();

#if true
                foreach (var song in controller.Songs)
                {
                    Console.WriteLine("{0}", song.Name);
                }
#else
                foreach (var midiProgram in programs.MidiPrograms)
                {
                    Console.WriteLine("midi: {0}", midiProgram.ProgramNumber);
                    foreach (var song in midiProgram.Songs) {
                        Console.WriteLine("  song: {0}", song.Name);
                    }
                }
#endif

                // Activate the first song:
                controller.ActivateSong(controller.Songs[0], 0);

                controller.ActivateScene(0);

                // Activate a new midi program's song:
                controller.ActivateSong(controller.MidiPrograms[1].Songs[0], 0);

                //new VGUI(controller).Run();
            }
#endif
        }
    }
}
