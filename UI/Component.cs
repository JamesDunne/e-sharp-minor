using System;
using e_sharp_minor;
using OpenVG;

namespace UI
{
    public abstract class Component : IDisposable
    {
        protected readonly IPlatform platform;
        protected readonly IOpenVG vg;
        protected readonly DisposalContainer disposalContainer;

        protected readonly float x, y, width, height;

        protected Component(IPlatform platform, float x, float y, float width, float height)
        {
            this.platform = platform;
            this.vg = platform.VG;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.disposalContainer = new DisposalContainer();
        }

        public void Dispose()
        {
            this.disposalContainer.Dispose();
        }

        public float X => x;
        public float Y => y;
        public float Width => width;
        public float Height => height;

        public abstract void Render();
    }
}
