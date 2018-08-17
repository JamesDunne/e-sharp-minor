using System;
using System.Diagnostics;
using System.Threading;
using VC;
using OpenVG;
using Shapes;

namespace e_sharp_minor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var bcmDisplay = new BcmDisplay(0))
                using (var dispmanxDisplay = bcmDisplay.CreateDispmanXDisplay())
                using (var vg = dispmanxDisplay.CreateOpenVGContext())
                {
                    Console.WriteLine("Display[0] = {0}x{1}", bcmDisplay.width, bcmDisplay.height);

                    vg.Setfv(ParamType.VG_CLEAR_COLOR, new float[] { 0.0f, 0.0f, 0.2f, 1.0f });

                    using (var strokePaint = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }))
                    using (var rect = new RoundRect(vg, 100, 100, bcmDisplay.width - 100 * 2, bcmDisplay.height - 100 * 2, 16, 16))
                    {
                        vg.Setf(ParamType.VG_STROKE_LINE_WIDTH, 1.0f);
                        // vgSeti(VG_STROKE_CAP_STYLE, ps->m_paths[i].m_capStyle);
                        // vgSeti(VG_STROKE_JOIN_STYLE, ps->m_paths[i].m_joinStyle);
                        // vgSetf(VG_STROKE_MITER_LIMIT, ps->m_paths[i].m_miterLimit);

                        // Render at 60fps for 5 seconds:
                        var sw = new Stopwatch();
                        for (int f = 0; f < 60 * 5; f++)
                        {
                            sw.Restart();

                            // Render our pre-made paths each frame:
                            vg.Clear(0, 0, (int)bcmDisplay.width, (int)bcmDisplay.height);

                            strokePaint.Activate(PaintMode.VG_STROKE_PATH);
                            rect.Render(null);

                            // Swap buffers to display and vsync:
                            vg.SwapBuffers();

                            // usually writes "15 ms"
                            Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
                        }
                    }

                    Console.WriteLine("Wait");

                    Thread.Sleep(5000);

                    Console.WriteLine("Shutdown");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
