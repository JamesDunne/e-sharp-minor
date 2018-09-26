﻿using System;
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
                    controller.ActivateSong(setlist.Songs[0], 0);

                    // Set up input event listener:
                    platform.InputEvent += (ev) =>
                    {
                        if (ev.TouchEvent.HasValue)
                        {
                            var touch = ev.TouchEvent.Value;
                            Console.WriteLine("{0},{1},{2}", touch.X, touch.Y, touch.Pressed);
                        }
                        else if (ev.FootSwitchEvent.HasValue)
                        {
                            FootSwitchEvent fsw = ev.FootSwitchEvent.Value;
                            Console.WriteLine("{0} {1}", fsw.FootSwitch, fsw.WhatAction);

                            if (fsw.FootSwitch == FootSwitch.Left)
                            {
                                if (controller.CurrentScene == 0)
                                {

                                }
                            }
                            else if (fsw.FootSwitch == FootSwitch.Right)
                            {

                            }
                        }
                    };

                    // Load TTF font:
                    NRasterizer.Typeface typeFace;
                    using (var fi = System.IO.File.OpenRead("Vera.ttf"))
                        typeFace = new NRasterizer.OpenTypeReader().Read(fi);

                    platform.VG.Seti(ParamType.VG_RENDERING_QUALITY, (int)RenderingQuality.VG_RENDERING_QUALITY_BETTER);
                    platform.VG.Seti(ParamType.VG_PIXEL_LAYOUT, (int)PixelLayout.VG_PIXEL_LAYOUT_RGB_HORIZONTAL);

                    var vera = platform.VG.CreateFont(typeFace.Glyphs.Count);
                    var vgRasterizer = new VGGlyphRasterizer(platform.VG);
                    vgRasterizer.ConvertGlyphs(typeFace, vera);

                    //platform.VG.DestroyFont(vera);
                    var white = new PaintColor(platform.VG, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });

                    // Initialize UI:
                    using (var ui = new VGUI(platform, controller))
                    {
                        bool quit = false;
                        do
                        {
                            platform.VG.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
                            platform.VG.Clear(0, 0, platform.FramebufferWidth, platform.FramebufferHeight);

                            // Render UI screen:
                            ui.Render();

                            // Test render some text:
                            platform.VG.FillPaint = white;
                            platform.VG.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE);
                            platform.VG.LoadIdentity();
                            platform.VG.Translate(220, 260);
                            platform.VG.Scale(20, 20);
                            platform.VG.Setfv(ParamType.VG_GLYPH_ORIGIN, new float[] { 0.0f, 0.0f });
                            platform.VG.DrawGlyphs(vera, "Step 1) Read Vera.ttf binary", PaintMode.VG_FILL_PATH, false);
                            platform.VG.Setfv(ParamType.VG_GLYPH_ORIGIN, new float[] { 0.0f, -1.0f });
                            platform.VG.DrawGlyphs(vera, "Step 2) Convert glyphs to OpenVG paths", PaintMode.VG_FILL_PATH, false);
                            platform.VG.Setfv(ParamType.VG_GLYPH_ORIGIN, new float[] { 0.0f, -2.0f });
                            platform.VG.DrawGlyphs(vera, "Step 3) Profit!", PaintMode.VG_FILL_PATH, false);

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