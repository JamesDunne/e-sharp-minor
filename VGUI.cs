using System;
using OpenVG;
using Shapes;

namespace e_sharp_minor
{
    public class VGUI : IDisposable
    {
        private readonly Controller controller;
        private readonly IPlatform platform;
        private readonly IOpenVG vg;
        private readonly DisposalContainer disposalContainer;
        private readonly PaintColor strokePaint;
        private readonly PaintColor fillPaint;
        private readonly RoundRect rect;

        public VGUI(IPlatform platform, Controller controller)
        {
            this.controller = controller;
            this.platform = platform;
            this.vg = platform.VG;

            Console.WriteLine("Display[0] = {0}x{1}", platform.Width, platform.Height);

            vg.ClearColor = new float[] { 0.0f, 0.0f, 0.2f, 1.0f };

            this.disposalContainer = new DisposalContainer(
                strokePaint = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }),
                fillPaint = new PaintColor(vg, new float[] { 0.6f, 0.6f, 0.6f, 1.0f }),
                rect = new RoundRect(vg, 100, 100, platform.Width - 100 * 2, platform.Height - 100 * 2, 16, 16)
                {
                    StrokeLineWidth = 1.0f
                }
            );
        }

        public void Dispose()
        {
            this.disposalContainer.Dispose();
        }

        public void Render()
        {
#if TIMING
            var sw = new Stopwatch();
            sw.Restart();
#endif

            // Render our pre-made paths each frame:
            vg.Clear(0, 0, platform.FramebufferWidth, platform.FramebufferHeight);

            vg.StrokePaint = strokePaint;
            vg.FillPaint = fillPaint;
            rect.Render(PaintMode.VG_FILL_PATH | PaintMode.VG_STROKE_PATH);

            // Swap buffers to display and vsync:
            platform.SwapBuffers();

#if TIMING
            // usually writes "16 ms"
            Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
#endif
        }
    }
}
