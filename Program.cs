using System;
using System.Diagnostics;
using System.Threading;
using VC;
using OpenVG;

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

                    var strokePaint = vg.CreatePaint();
                    vg.SetParameteri(strokePaint, (int)PaintParamType.VG_PAINT_TYPE, (int)PaintType.VG_PAINT_TYPE_COLOR);
                    vg.SetParameterfv(strokePaint, (int)PaintParamType.VG_PAINT_COLOR, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });

                    // Set up rounded rectangle path:
                    var path = vg.CreatePath(Constants.VG_PATH_FORMAT_STANDARD, PathDatatype.VG_PATH_DATATYPE_F, 1.0f, 0.0f, 0, 0, PathCapabilities.VG_PATH_CAPABILITY_ALL);
                    vg.Setf(ParamType.VG_STROKE_LINE_WIDTH, 1.0f);
                    // vgSeti(VG_STROKE_CAP_STYLE, ps->m_paths[i].m_capStyle);
                    // vgSeti(VG_STROKE_JOIN_STYLE, ps->m_paths[i].m_joinStyle);
                    // vgSetf(VG_STROKE_MITER_LIMIT, ps->m_paths[i].m_miterLimit);
                    VGU.RoundRect(path, 100, 100, bcmDisplay.width - 100 * 2, bcmDisplay.height - 100 * 2, 16, 16);

                    // Render at 60fps for 5 seconds:
                    var sw = new Stopwatch();
                    for (int f = 0; f < 60 * 5; f++)
                    {
                        sw.Restart();

                        // Render our pre-made paths each frame:
                        vg.Clear(0, 0, (int)bcmDisplay.width, (int)bcmDisplay.height);

                        vg.SetPaint(strokePaint, PaintMode.VG_STROKE_PATH);
                        vg.DrawPath(path, PaintMode.VG_STROKE_PATH);

                        // Swap buffers to display and vsync:
                        vg.SwapBuffers();

                        // usually writes "15 ms"
                        Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
                    }

                    vg.DestroyPath(path);
                    vg.DestroyPaint(strokePaint);

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
