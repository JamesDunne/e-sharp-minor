using System;
using System.Threading;
using VC;

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
                using (var eglContext = dispmanxDisplay.CreateEGLContext())
                {
                    Console.WriteLine("Display[0] = {0}x{1}", bcmDisplay.width, bcmDisplay.height);

                    VG.Setfv(VG.ParamType.VG_CLEAR_COLOR, 4, new float[] { 0.0f, 0.0f, 0.2f, 1.0f });
                    VG.Clear(0, 0, (int)bcmDisplay.width, (int)bcmDisplay.height);

                    var strokePaint = VG.CreatePaint();
                    VG.SetPaintParameteri(strokePaint, VG.PaintParamType.VG_PAINT_TYPE, (int)VG.PaintType.VG_PAINT_TYPE_COLOR);
                    VG.SetPaintParameterfv(strokePaint, VG.PaintParamType.VG_PAINT_COLOR, 4, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });

                    var path = VG.CreatePath(VG.VG_PATH_FORMAT_STANDARD, VG.PathDatatype.VG_PATH_DATATYPE_F, 1.0f, 0.0f, 0, 0, VG.PathCapabilities.VG_PATH_CAPABILITY_ALL);
                    VG.Setf(VG.ParamType.VG_STROKE_LINE_WIDTH, 1.0f);
                    // vgSeti(VG_STROKE_CAP_STYLE, ps->m_paths[i].m_capStyle);
                    // vgSeti(VG_STROKE_JOIN_STYLE, ps->m_paths[i].m_joinStyle);
                    // vgSetf(VG_STROKE_MITER_LIMIT, ps->m_paths[i].m_miterLimit);
                    VG.SetPaint(strokePaint, VG.PaintMode.VG_STROKE_PATH);
                    VGU.RoundRect(path, 200, 200, 200, 200, 12, 12);
                    VG.DrawPath(path, VG.PaintMode.VG_STROKE_PATH);

                    eglContext.SwapBuffers();

                    // DestroyPath
                    // DestroyPaint

                    Thread.Sleep(1000);

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
