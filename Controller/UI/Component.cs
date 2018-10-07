using System;
using Shapes;
using OpenVG;

namespace EMinor.UI
{
    public abstract class Component : IDisposable
    {
        protected readonly IPlatform platform;
        protected readonly IOpenVG vg;
        protected readonly DisposalContainer disposalContainer;

        protected readonly Point point;
        protected readonly Bounds bounds;

        protected Component(IPlatform platform, Point point, Bounds bounds)
        {
            this.platform = platform;
            this.vg = platform.VG;
            this.point = point;
            this.bounds = bounds;
            this.disposalContainer = new DisposalContainer();
        }

        public void Dispose()
        {
            this.disposalContainer.Dispose();
        }

        public Point Point => point;
        public Bounds Bounds => bounds;

        public abstract void Render();

        public bool IsPointInside(Point p) => p.X >= point.X && p.Y >= point.Y && p.X < point.X + bounds.W && p.Y < point.Y + bounds.H;
    }
}
