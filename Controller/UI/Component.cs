using System;
using System.Collections.Generic;

using Shapes;
using OpenVG;
using System.Collections.ObjectModel;

namespace EMinor.UI
{
    public abstract class Component : IDisposable
    {
        protected readonly IPlatform platform;
        protected readonly IOpenVG vg;
        protected readonly DisposalContainer disposalContainer;

        private readonly List<Component> children;

        protected Component(IPlatform platform)
        {
            this.platform = platform;
            this.vg = platform.VG;
            this.disposalContainer = new DisposalContainer();
            this.children = new List<Component>();
        }

        public void Dispose()
        {
            this.disposalContainer.Dispose();
            this.children.ForEach(c => c.Dispose());
        }

        private void SetParent(Component component)
        {
            this.Parent = component;
        }

        public virtual IList<Component> Children
        {
            get { return this.children; }
            set
            {
                this.children.Clear();
                this.children.AddRange(value);
                this.children.ForEach(c => c.SetParent(this));
            }
        }
        public Component Parent { get; private set; }

        public Dock Dock { get; set; }

        public Point? ExplicitPoint { get; set; }
        public Bounds? ExplicitBounds { get; set; }

        public Point? ComputedPoint { get; set; }
        public Bounds? ComputedBounds { get; set; }

        public Point Point => ExplicitPoint ?? ComputedPoint ?? EMinor.Point.Zero;
        public Bounds Bounds => ExplicitBounds ?? ComputedBounds ?? Parent?.Bounds ?? new Bounds(0, 0);

        public abstract void CalculateChildrenLayout();

        public virtual void Render()
        {
            vg.PushMatrix();

            // Translate to point relative to parent:
            vg.Translate(this.Point.X, this.Point.Y);
            this.RenderSelf();

            foreach (var child in Children)
            {
                child.Render();
            }
            vg.PopMatrix();
        }

        public abstract void RenderSelf();

        public Action OnPress { get; set; }

        public bool IsPointInside(Point p) => p.X >= Point.X && p.Y >= Point.Y && p.X < Point.X + Bounds.W && p.Y < Point.Y + Bounds.H;
    }

    public enum Dock
    {
        None,
        Fill,
        Left,
        Top,
        Right,
        Bottom
    }
}
