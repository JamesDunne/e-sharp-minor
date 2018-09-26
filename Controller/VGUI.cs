using System;
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

        public VGUI(IPlatform platform, Controller controller)
        {
            this.controller = controller;
            this.platform = platform;
            this.vg = platform.VG;

            Console.WriteLine("Display[0] = {0}x{1} ({2}x{3})", platform.Width, platform.Height, platform.FramebufferWidth, platform.FramebufferHeight);

            vg.ClearColor = new float[] { 0.0f, 0.0f, 0.2f, 1.0f };

            this.disposalContainer = new DisposalContainer(
                root = new Panel(platform, new Bounds(platform.Width, platform.Height)),
                strokePaint = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }),
                fillPaint = new PaintColor(vg, new float[] { 0.6f, 0.6f, 0.6f, 1.0f }),
                btn = new Button(platform, new Bounds(100, 100, platform.Width - 100 * 2, platform.Height - 100 * 2))
                {
                    Stroke = strokePaint,
                    Fill = fillPaint
                }
            );

            platform.InputEvent += Platform_InputEvent;
        }

        void Platform_InputEvent(InputEvent @event)
        {
            if (!@event.TouchEvent.HasValue) return;

            var touch = @event.TouchEvent.Value;
        }

        public void Dispose()
        {
            this.disposalContainer.Dispose();
        }

        public void Render()
        {
            // Render our pre-made paths each frame:
            btn.Render();
        }
    }
}
