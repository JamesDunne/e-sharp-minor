﻿using System;
using System.Diagnostics;
using System.Threading;
using OpenVG;
using Shapes;
#if RPI
using VC;
#elif OSX
using Amanith;
#endif

namespace e_sharp_minor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
#if RPI
                using (var bcmDisplay = new BcmDisplay(0))
                using (var dispmanxDisplay = bcmDisplay.CreateDispmanXDisplay())
                using (var vg = dispmanxDisplay.CreateOpenVGContext())
#else
                using (var vg = new OpenVGContext(800, 480))
#endif
                {
                    Draw(vg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Draw(IOpenVG vg)
        {
            Console.WriteLine("Display[0] = {0}x{1}", vg.Width, vg.Height);

            vg.Setfv(ParamType.VG_CLEAR_COLOR, new float[] { 0.0f, 0.0f, 0.2f, 1.0f });

            using (var strokePaint = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }))
            using (var fillPaint = new PaintColor(vg, new float[] { 0.6f, 0.6f, 0.6f, 1.0f }))
            using (var rect = new RoundRect(vg, 100, 100, vg.Width - 100 * 2, vg.Height - 100 * 2, 16, 16)
            {
                StrokeLineWidth = 2.0f
                // vgSeti(VG_STROKE_CAP_STYLE, ps->m_paths[i].m_capStyle);
                // vgSeti(VG_STROKE_JOIN_STYLE, ps->m_paths[i].m_joinStyle);
                // vgSetf(VG_STROKE_MITER_LIMIT, ps->m_paths[i].m_miterLimit);
            })
            {
                // Render at 60fps for 5 seconds:
                var sw = new Stopwatch();
                for (int f = 0; f < 60 * 5; f++)
                {
                    sw.Restart();

                    // Render our pre-made paths each frame:
                    vg.Clear(0, 0, vg.Width, vg.Height);

                    strokePaint.Activate(PaintMode.VG_STROKE_PATH);
                    fillPaint.Activate(PaintMode.VG_FILL_PATH);
                    rect.Render(PaintMode.VG_FILL_PATH | PaintMode.VG_STROKE_PATH);

                    // Swap buffers to display and vsync:
                    vg.SwapBuffers();

                    // usually writes "16 ms"
                    Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
                }
            }

            Console.WriteLine("Wait");

            Thread.Sleep(5000);

            Console.WriteLine("Shutdown");
        }
    }
}
