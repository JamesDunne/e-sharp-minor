﻿using System;
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

                Console.WriteLine("alphabetical:");
                foreach (var song in controller.Songs)
                {
                    Console.WriteLine("  {0}", song.Name);
                }

                Console.WriteLine();
                foreach (var midiProgram in controller.MidiPrograms)
                {
                    Console.WriteLine("midi: {0}", midiProgram.ProgramNumber);
                    foreach (var song in midiProgram.Songs)
                    {
                        Console.WriteLine("  song: {0}", song.Name);
                    }
                }
                Console.WriteLine();

                // Activate the first song:
                controller.ActivateSong(controller.Songs[0], 0);

                controller.ActivateScene(0);

                // Activate a new midi program's song:
                controller.ActivateSong(controller.MidiPrograms[1].Songs[0], 0);

#if RPI
                using (var fsw = new FootSwitchInput())
                {
                    bool quit = false;
                    do
                    {
                        bool? left, right;
                        if (fsw.PollFootSwitches(out left, out right))
                        {
                            if (left.HasValue) Console.WriteLine("left button {0}", left.Value ? "pressed" : "released");
                            if (right.HasValue) Console.WriteLine("right button {0}", right.Value ? "pressed" : "released");
                        }
                        else
                        {
                            Console.WriteLine("No button events");
                        }
                    } while (!quit);
                }
#endif

                //new VGUI(controller).Run();
            }
#endif
        }
    }
}
