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

                    eglContext.SwapBuffers();
                    VG.Setfv(VG.ParamType.VG_CLEAR_COLOR, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                    VG.Clear(0, 0, (int)bcmDisplay.width, (int)bcmDisplay.height);

                    var path = VG.CreatePath(VG.VG_PATH_FORMAT_STANDARD, VG.PathDatatype.VG_PATH_DATATYPE_F, 1.0f, 0.0f, -1, -1, VG.PathCapabilities.VG_PATH_CAPABILITY_ALL);
                    VGU.RoundRect(path, 200, 200, 200, 200, 12, 12);

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
