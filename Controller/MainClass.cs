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
            if (args.Length != 0 && args[0] == "translate")
            {
                Console.WriteLine("Running v5 to v6 translator...");
                var translator = new Translator();
                translator.Translate();
                return;
            }

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
                    Console.WriteLine($"Display[0] = {platform.Width}x{platform.Height} ({platform.FramebufferWidth}x{platform.FramebufferHeight})");

                    var controller = new Controller(platform.MIDI, channel: 2);
                    controller.LoadData();

                    Console.WriteLine();
                    foreach (var midiProgram in controller.MidiPrograms)
                    {
                        Console.WriteLine("midi: {0}", midiProgram.ProgramNumber);
                        foreach (var song in midiProgram.Songs)
                        {
                            Console.WriteLine("  {0}", song.Name);
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("all songs alphabetical:");
                    for (int i = 0; i < controller.Songs.Count; i++)
                    {
                        var song = controller.Songs[i];
                        Console.WriteLine("  {0}. {1}", i + 1, song.Name);
                    }
                    Console.WriteLine();

                    // Use latest active setlist:
                    var setlist = (
                        from sl in controller.Setlists
                        where sl.Active
                        select sl
                    ).Last();

                    Console.WriteLine("Setlist for {0} on {1}", setlist.Venue, setlist.Date);
                    Console.WriteLine("{0} songs", setlist.Songs.Count);
                    for (int i = 0; i < setlist.Songs.Count; i++)
                    {
                        var song = setlist.Songs[i];
                        Console.WriteLine("  {0}. {1}", i + 1, song.Name);
                    }

                    // Activate the first song in the setlist:
                    controller.StartMidiBatch();
                    controller.ActivateSetlist(setlist);
                    controller.EndMidiBatch();

                    // Initialize UI:
                    IOpenVG vg = platform.VG;
                    // Start a new thread to handle rendering:
                    var renderThread = platform.NewRenderThread(() =>
                    {
                        using (var ui = new VGUI(platform, controller))
                        {
#if TIMING
                            var sw = new Stopwatch();
                            sw.Start();
#endif
                            while (true)
                            {
#if TIMING
                                double start = sw.Elapsed.TotalMilliseconds;
#endif
                                // Render UI screen:
                                ui.Render();

#if TIMING
                                Console.Out.WriteLineAsync($"{sw.Elapsed.TotalMilliseconds - start:N2} ms");
#endif

                                // Wait for next frame:
                                ui.EndFrame();
                            }
                        }
                    });
                    renderThread.Start();

                    // Main thread:
                    bool quit = false;
                    do
                    {
                        platform.WaitEvents();

                        // Check with the GUI if user indicated app should quit:
                        quit |= platform.ShouldQuit();
                    } while (!quit);
                }
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex);
            }
        }
    }
}
