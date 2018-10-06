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
        private readonly Button btn;
        private readonly Component root;
        private PaintColor white;
        private Ellipse point;
        private FontHandle vera;

        public VGUI(IPlatform platform, Controller controller)
        {
            this.controller = controller;
            this.platform = platform;
            vg = platform.VG;

            Console.WriteLine("Display[0] = {0}x{1} ({2}x{3})", platform.Width, platform.Height, platform.FramebufferWidth, platform.FramebufferHeight);

            vg.ClearColor = new float[] { 0.0f, 0.0f, 0.2f, 1.0f };

            this.disposalContainer = new DisposalContainer(
                root = new Panel(platform, new Point(0, 0), new Bounds(platform.Width, platform.Height)),
                strokePaint = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }),
                fillPaint = new PaintColor(vg, new float[] { 0.6f, 0.6f, 0.6f, 1.0f }),
                btn = new Button(
                    platform,
                    new Point(100, 100),
                    new RoundRect(vg, new Bounds(platform.Width - 100 * 2, platform.Height - 100 * 2), 16, 16)
                    {
                        StrokeLineWidth = 1.0f
                    }
                )
                {
                    Stroke = strokePaint,
                    Fill = fillPaint
                }
            );

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

            this.white = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            this.point = new Ellipse(vg, 24, 24);
        }

        TouchEvent touch = new TouchEvent
        {
            X = 0,
            Y = 0,
            Pressed = false
        };

        void Platform_InputEvent(InputEvent ev)
        {
            if (ev.TouchEvent.HasValue)
            {
                touch = ev.TouchEvent.Value;
                //Console.WriteLine("{0},{1},{2}", touch.X, touch.Y, touch.Pressed);
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
        }

        public void Dispose()
        {
            platform.VG.DestroyFont(vera);
            this.disposalContainer.Dispose();
        }

        public void Render()
        {
            // Render our pre-made paths each frame:
            btn.Render();

            // Test render some text:
            vg.FillPaint = white;
            vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_GLYPH_USER_TO_SURFACE);
            vg.PushMatrix();
            vg.Translate(220, 260);
            vg.Scale(18, 18);
            vg.Setfv(ParamType.VG_GLYPH_ORIGIN, new float[] { 0.0f, 0.0f });
            vg.DrawGlyphs(vera, "Step 1) Read Vera.ttf binary", PaintMode.VG_FILL_PATH, false);
            vg.Setfv(ParamType.VG_GLYPH_ORIGIN, new float[] { 0.0f, -1.0f });
            vg.DrawGlyphs(vera, "Step 2) Convert glyphs to OpenVG paths", PaintMode.VG_FILL_PATH, false);
            vg.Setfv(ParamType.VG_GLYPH_ORIGIN, new float[] { 0.0f, -2.0f });
            vg.DrawGlyphs(vera, "Step 3) Profit!", PaintMode.VG_FILL_PATH, false);
            vg.PopMatrix();

            vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);

            // Draw touch cursor:
            if (touch.Pressed)
            {
                vg.FillPaint = white;
                vg.PushMatrix();
                vg.Translate(touch.X, touch.Y);
                point.Render(PaintMode.VG_FILL_PATH);
                vg.PopMatrix();
            }

        }
    }
}
