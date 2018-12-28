#define TIMING
#define THREADS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenVG;

namespace EMinor
{
    class MainClass
    {
        // assuming 60Hz, 10 seconds:
        const int totalBenchmarkPoints = 60 * 10;

        static void Main(string[] args)
        {
#if !TRANSLATOR
            bool benchmark = false;

            if (args.Length != 0)
            {
                if (args[0] == "translate")
                {
#endif
                    Console.WriteLine("Running v5 to v6 translator...");
                    var translator = new Translator();
                    translator.Translate();
#if !TRANSLATOR
                    return;
                }
                else if (args[0] == "benchmark")
                {
                    Console.WriteLine("Benchmark mode ON");
                    benchmark = true;
                }
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

                    List<double> benchmarkPoints = null;
                    if (benchmark)
                    {
                        benchmarkPoints = new List<double>(totalBenchmarkPoints);
                    }

                    VGUI ui = null;
#if THREADS
                    ManualResetEvent uiInitialized = new ManualResetEvent(false);

                    // Start a new thread to handle rendering:
                    var renderThread = platform.NewRenderThread(() =>
                    {
                        using (ui = new VGUI(platform, controller))
                        {
                            uiInitialized.Set();

                            if (benchmark)
                            {
                                var sw = new Stopwatch();
                                sw.Start();

                                while (true)
                                {
                                    double start = sw.Elapsed.TotalMilliseconds;

                                    // Render UI screen:
                                    ui.Render();

                                    var elapsed = sw.Elapsed.TotalMilliseconds - start;
                                    benchmarkPoints.Add(elapsed);

                                    ui.FrameReady();
#if TIMING
                                    Console.Out.WriteLineAsync($"{elapsed:N2} ms");
#endif

                                    // Wait for next frame:
                                    ui.WaitForNextFrame();
                                }
                            }
                            else
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
                                    ui.WaitForNextFrame();
                                }
                            }
                        }
                    });
                    renderThread.Start();

                    // Wait for renderer thread to initialize:
                    uiInitialized.WaitOne();
                    if (benchmark)
                    {
                        // Toss out 20 frames to warm up JIT:
                        for (int i = 0; i < 20; i++)
                        {
                            platform.PollEvents();
                            ui.AllowFrame();
                            ui.WaitForFrameReady();
                        }
                        benchmarkPoints.Clear();
                        // Start the benchmark:
                        Console.WriteLine("Benchmark started");
                        for (int i = 0; i < totalBenchmarkPoints; i++)
                        {
                            platform.PollEvents();
                            ui.AllowFrame();
                            ui.WaitForFrameReady();
                        }
                        Console.WriteLine("Benchmark complete");
                        Console.WriteLine($"Min   time: {benchmarkPoints.Min():N2} ms");
                        Console.WriteLine($"Max   time: {benchmarkPoints.Max():N2} ms");
                        Console.WriteLine($"Avg   time: {benchmarkPoints.Average():N2} ms");
                        Console.WriteLine($"Total time: {benchmarkPoints.Sum():N2} ms");
                        return;
                    }

                    // Main thread:
                    bool quit = false;
                    do
                    {
                        platform.WaitEvents();

                        // Check with the GUI if user indicated app should quit:
                        quit |= platform.ShouldQuit();
                    } while (!quit);
#else
                    // Non-threaded version:
                    platform.InitRenderThread();
                    using (ui = new VGUI(platform, controller))
                    {
                        Stopwatch sw;

                        if (benchmark)
                        {
                            // Toss out 20 frames to warm up JIT:
                            for (int i = 0; i < 20; i++)
                            {
                                ui.Render();
                                platform.PollEvents();
                            }

                            // Start the benchmark:
                            Console.WriteLine("Benchmark started");
                            sw = new Stopwatch();
                            sw.Start();
                            for (int i = 0; i < totalBenchmarkPoints; i++)
                            {
                                double start = sw.Elapsed.TotalMilliseconds;

                                ui.Render();
                                platform.PollEvents();

                                var elapsed = sw.Elapsed.TotalMilliseconds - start;
                                benchmarkPoints.Add(elapsed);
#if TIMING
                                Console.Out.WriteLineAsync($"{elapsed:N2} ms");
#endif
                            }

                            Console.WriteLine("Benchmark complete");
                            Console.WriteLine($"Min   time: {benchmarkPoints.Min():N2} ms");
                            Console.WriteLine($"Max   time: {benchmarkPoints.Max():N2} ms");
                            Console.WriteLine($"Avg   time: {benchmarkPoints.Average():N2} ms");
                            Console.WriteLine($"Total time: {benchmarkPoints.Sum():N2} ms");
                            return;
                        }

                        // Main thread:
                        bool quit = false;
#if TIMING
                        sw = new Stopwatch();
                        sw.Start();
#endif
                        do
                        {
#if TIMING
                            double start = sw.Elapsed.TotalMilliseconds;
#endif
                            ui.Render();

#if TIMING
                            Console.Out.WriteLineAsync($"{sw.Elapsed.TotalMilliseconds - start:N2} ms");
#endif

                            platform.WaitEvents();

                            // Check with the GUI if user indicated app should quit:
                            quit |= platform.ShouldQuit();
                        } while (!quit);
                    }
#endif
                }
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex);
            }
#endif
        }
    }
}
