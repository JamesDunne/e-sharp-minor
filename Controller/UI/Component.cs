using System;
using EMinor;
using OpenVG;

namespace EMinor.UI
{
    public abstract class Component : IDisposable
    {
        protected readonly IPlatform platform;
        protected readonly IOpenVG vg;
        protected readonly DisposalContainer disposalContainer;

        protected readonly Bounds bounds;

        protected Component(IPlatform platform, Bounds bounds)
        {
            this.platform = platform;
            this.vg = platform.VG;
            this.bounds = bounds;
            this.disposalContainer = new DisposalContainer();
        }

        public void Dispose()
        {
            this.disposalContainer.Dispose();
        }

        public Bounds Bounds => bounds;

        public abstract void Render();
    }
}
