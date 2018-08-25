using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenVG;
using Shapes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// Select appropriate device implementation classes depending on build configuration
// Use 'pi-debug' configuration to enable RPI.
#if RPI
using MidiOut = e_sharp_minor.MidiAlsaOut;
using FootSwitchInput = e_sharp_minor.FootSwitchInputEvdev;
#else
using MidiOut = e_sharp_minor.MidiConsoleOut;
using FootSwitchInput = e_sharp_minor.FootSwitchInputConsole;
#endif

namespace e_sharp_minor
{
    class MainClass
    {
        static void Main(string[] args)
        {
#if false
            var translator = new Translator();
            translator.Translate();
#endif

            // Initialize devices:
            using (IMIDI midi = new MidiOut())
            using (IFootSwitchInput fsw = new FootSwitchInput())
            {
                var controller = new Controller(midi, 2);
                controller.LoadData();

                Console.WriteLine("all songs alphabetical:");
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

                var setlist = (
                    from sl in controller.Setlists
                    where sl.Active
                    select sl
                ).Last();

                // Match song names with songs:
                setlist.Songs = new List<V6.Song>(setlist.SongNames.Count);
                for (int i = 0; i < setlist.SongNames.Count; i++)
                {
                    var setlistSongName = setlist.SongNames[i];
                    if (setlistSongName.StartsWith("BREAK:", StringComparison.OrdinalIgnoreCase)) continue;

                    setlist.Songs.Add((
                        from song in controller.Songs
                        where song.MatchesName(setlistSongName)
                        select song
                    ).Single());
                }

                Console.WriteLine("Setlist for {0} on {1}", setlist.Venue, setlist.Date);
                Console.WriteLine("{0} songs", setlist.Songs.Count);
                foreach (var song in setlist.Songs)
                {
                    Console.WriteLine("  {0}", song.Name);
                }

                // Activate the first song in the setlist:
                controller.ActivateSong(setlist.Songs[0], 0);

                // Set up footswitch event listener:
                fsw.EventListener += (ev) =>
                {
                    Console.WriteLine("{0} {1}", ev.FootSwitch, ev.WhatAction);

                    if (ev.FootSwitch == FootSwitch.Left)
                    {
                        if (controller.CurrentScene == 0)
                        {

                        }
                    }
                    else if (ev.FootSwitch == FootSwitch.Right)
                    {

                    }
                };

                // Initialize UI:
                using (var ui = new VGUI(controller))
                {
                    bool quit = false;
                    do
                    {
                        // TODO: switch to blocking wait for all events.
                        fsw.PollEvents();

                        // Render UI screen:
                        ui.Render();

                        // Check with the GUI if user indicated app should quit:
                        quit |= ui.ShouldQuit();
                    } while (!quit);
                }
            }
        }
    }
}
