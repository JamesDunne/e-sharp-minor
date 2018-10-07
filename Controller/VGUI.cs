using System;
using System.Diagnostics;
using EMinor.UI;
using OpenVG;
using Shapes;

namespace EMinor
{
    public class VGUI : IDisposable
    {
        private readonly Controller controller;
        private readonly IPlatform platform;
        private readonly IOpenVG vg;
        private readonly DisposalContainer disposalContainer;

        private readonly PaintColor strokePaint;
        private readonly PaintColor fillPaint;
        private readonly Button btnSong;
        private readonly Component root;
        private readonly PaintColor white;
        private readonly PaintColor pointColor;
        private readonly Ellipse point;
        private readonly FontHandle vera;
        private readonly Button btnScene;

        public VGUI(IPlatform platform, Controller controller)
        {
            this.controller = controller;
            this.platform = platform;
            vg = platform.VG;

            vg.ClearColor = new float[] { 0.0f, 0.0f, 0.2f, 1.0f };

            platform.InputEvent += Platform_InputEvent;

            // Load TTF font:
            Debug.WriteLine("Load Vera.ttf");
            NRasterizer.Typeface typeFace;
            using (var fi = System.IO.File.OpenRead("Vera.ttf"))
            {
                Debug.WriteLine("OpenTypeReader");
                typeFace = new NRasterizer.OpenTypeReader().Read(fi);
            }

            Debug.WriteLine("Set rendering quality and pixel layout");
            vg.Seti(ParamType.VG_RENDERING_QUALITY, (int)RenderingQuality.VG_RENDERING_QUALITY_BETTER);
            vg.Seti(ParamType.VG_PIXEL_LAYOUT, (int)PixelLayout.VG_PIXEL_LAYOUT_RGB_HORIZONTAL);

            vera = vg.CreateFont(typeFace.Glyphs.Count);
            var vgRasterizer = new VGGlyphRasterizer(vg);
            vgRasterizer.ConvertGlyphs(typeFace, vera);

            this.disposalContainer = new DisposalContainer(
                white = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }),
                // For the touch point:
                pointColor = new PaintColor(vg, new float[] { 0.0f, 1.0f, 0.0f, 0.5f }),
                point = new Ellipse(vg, 24, 24),
                // Root panel bounds:
                root = new Panel(platform, new Point(0, 0), new Bounds(platform.Width, platform.Height)),
                strokePaint = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }),
                fillPaint = new PaintColor(vg, new float[] { 0.3f, 0.3f, 0.3f, 1.0f }),
                btnSong = new Button(
                    platform,
                    new Point(0, platform.Height - 32),
                    new RoundRect(vg, new Bounds(platform.Width - 80, 32), 16, 16),
                    vera,
                    white,
                    () => controller.CurrentSongName
                )
                {
                    Stroke = strokePaint,
                    Fill = fillPaint
                },
                btnScene = new Button(
                    platform,
                    new Point(platform.Width - 80, platform.Height - 32),
                    new RoundRect(vg, new Bounds(80, 32), 16, 16),
                    vera,
                    white,
                    () => controller.CurrentSceneDisplay
                )
                {
                    Stroke = strokePaint,
                    Fill = fillPaint
                }
            );
        }

        TouchEvent touch = new TouchEvent
        {
            Point = new Point(0, 0),
            Pressed = false
        };

        void Platform_InputEvent(InputEvent ev)
        {
            if (ev.TouchEvent.HasValue)
            {
                // Record last touch point:
                touch = ev.TouchEvent.Value;

                //Console.WriteLine("{0},{1},{2}", touch.X, touch.Y, touch.Pressed);
            }
            else if (ev.FootSwitchEvent.HasValue)
            {
                FootSwitchEvent fsw = ev.FootSwitchEvent.Value;
                //Console.WriteLine("{0} {1}", fsw.FootSwitch, fsw.WhatAction);

                if (fsw.WhatAction != FootSwitchAction.Released)
                {
                    if (fsw.FootSwitch == FootSwitch.Left)
                    {
                        controller.PreviousScene();
                    }
                    else if (fsw.FootSwitch == FootSwitch.Right)
                    {
                        controller.NextScene();
                    }
                }
            }
        }

        public void Dispose()
        {
            platform.VG.DestroyFont(vera);
            this.disposalContainer.Dispose();
        }

        public void Render()
        {
            // Render our pre-made paths each frame:
            if (touch.Pressed && btnSong.IsPointInside(touch.Point))
            {

            }
            btnSong.Render();
            btnScene.Render();

            // Draw touch cursor:
            if (touch.Pressed)
            {
                vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
                vg.PushMatrix();
                vg.Translate(touch.Point.X, touch.Point.Y);
                vg.FillPaint = pointColor;
                point.Render(PaintMode.VG_FILL_PATH);
                vg.PopMatrix();
            }
        }
    }
}
