using System;
using OpenVG;
using Shapes;

namespace e_sharp_minor
{
    public class VGUI
    {
        private readonly Controller controller;
        private readonly IOpenVG vg;

        public VGUI(Controller controller)
        {
            this.controller = controller;

#if RPI
            this.vg = new OpenVGContext(0);
#else
            this.vg = new OpenVGContext(800, 480);
#endif
        }

        public void Run()
        {
            using (vg)
            {
                Console.WriteLine("Display[0] = {0}x{1}", vg.Width, vg.Height);

                vg.ClearColor = new float[] { 0.0f, 0.0f, 0.2f, 1.0f };

                PaintColor strokePaint;
                PaintColor fillPaint;
                RoundRect rect;

                using (new DisposalContainer(
                    strokePaint = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }),
                    fillPaint = new PaintColor(vg, new float[] { 0.6f, 0.6f, 0.6f, 1.0f }),
                    rect = new RoundRect(vg, 100, 100, vg.Width - 100 * 2, vg.Height - 100 * 2, 16, 16)
                    {
                        StrokeLineWidth = 1.0f
                    }
                ))
                {
#if TIMING
                    // Render at 60fps for 15 seconds:
                    var sw = new Stopwatch();
#endif
                    for (int f = 0; f < 60 * 15; f++)
                    {
#if TIMING
                        sw.Restart();
#endif
                        // Render our pre-made paths each frame:
                        vg.Clear(0, 0, vg.Width, vg.Height);

                        vg.StrokePaint = strokePaint;
                        vg.FillPaint = fillPaint;
                        rect.Render(PaintMode.VG_FILL_PATH | PaintMode.VG_STROKE_PATH);

                        // Swap buffers to display and vsync:
                        vg.SwapBuffers();

#if TIMING
                        // usually writes "16 ms"
                        Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
#endif
                    }
                }

                Console.WriteLine("Shutdown");
            }
        }
    }
}
