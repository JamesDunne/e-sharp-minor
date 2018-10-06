using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenVG;
using Shapes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace EMinor
{
    class MainClass
    {
        static void Main(string[] args)
        {
#if false
            var translator = new Translator();
            translator.Translate();
#endif

            try
            {
                // Select appropriate device implementation classes depending on build configuration
                // Use 'pi-debug' configuration to enable RPI.
#if RPI
                Console.WriteLine("RPI platform");
                using (IPlatform platform = new RpiPlatform(0))
#else
                Console.WriteLine("GLFW platform");
                using (IPlatform platform = new GlfwPlatform(800, 480))
#endif
                {
                    var controller = new Controller(platform.MIDI, channel: 2);
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
                    controller.CurrentSetlist = setlist;
                    controller.ActivateSong(setlist.Songs[0], 0);

                    // Initialize UI:
                    IOpenVG vg = platform.VG;
                    using (var ui = new VGUI(platform, controller))
                    {
                        bool quit = false;
                        do
                        {
                            vg.Clear(0, 0, platform.FramebufferWidth, platform.FramebufferHeight);

                            // Render UI screen:
                            ui.Render();

                            // Swap buffers to display and vsync:
                            platform.SwapBuffers();

                            // Wait for next event:
                            platform.WaitEvents();

                            // Check with the GUI if user indicated app should quit:
                            quit |= platform.ShouldQuit();
                        } while (!quit);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex);
            }
        }
    }
}
