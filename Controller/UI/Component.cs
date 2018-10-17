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

        private readonly List<Component> children;

        protected Component(IPlatform platform)
        {
            this.platform = platform;
            this.vg = platform.VG;
            this.children = new List<Component>();
        }

        public virtual void Dispose()
        {
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

        public float? Width { get; set; }
        public float? Height { get; set; }

        public Point? ComputedPoint { get; set; }
        public Bounds? ComputedBounds { get; set; }

        public Point Point => ComputedPoint ?? EMinor.Point.Zero;
        public Bounds Bounds => ComputedBounds ?? Parent?.Bounds ?? platform.Bounds;

        protected virtual void CalculateChildrenLayout(Point point, Bounds bounds, List<Component> fillChildren)
        {
            // Naive fill implementation that merely overlaps all children:
            foreach (var child in fillChildren)
            {
                child.ComputedPoint = Point.Zero;
                child.ComputedBounds = bounds;
                child.CalculateLayout();
            }
        }

        protected virtual void CreateShape()
        {
            // Override me to create OpenVG paths for the component.
        }

        public void CalculateLayout()
        {
            CreateShape();

            Point point = Point.Zero;
            Bounds bounds = Bounds;
            var fillChildren = new List<Component>(Children.Count);

            // Process docked children first and close up outer bounds:
            foreach (var child in Children)
            {
                if (child.Dock == Dock.Fill)
                {
                    fillChildren.Add(child);
                    continue;
                }

                if (child.Dock == Dock.Top)
                {
                    bounds.H -= child.Height.Value;
                    child.ComputedPoint = point + new Point(0, bounds.H);
                    child.ComputedBounds = new Bounds(bounds.W, child.Height.Value);
                }
                else if (child.Dock == Dock.Right)
                {
                    bounds.W -= child.Width.Value;
                    child.ComputedPoint = point + new Point(bounds.W, 0);
                    child.ComputedBounds = new Bounds(child.Width.Value, bounds.H);
                }
                else if (child.Dock == Dock.Bottom)
                {
                    bounds.H -= child.Height.Value;
                    child.ComputedPoint = point;
                    child.ComputedBounds = new Bounds(bounds.W, child.Height.Value);
                    point += new Point(0, child.Height.Value);
                }
                else if (child.Dock == Dock.Left)
                {
                    bounds.W -= child.Width.Value;
                    child.ComputedPoint = point;
                    child.ComputedBounds = new Bounds(child.Width.Value, bounds.H);
                    point += new Point(child.Width.Value, 0);
                }

                child.CalculateLayout();
            }

            // Fill in remaining children:
            if (fillChildren.Count > 0)
            {
                CalculateChildrenLayout(point, bounds, fillChildren);
            }
        }

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

        protected abstract void RenderSelf();

        protected virtual void BeforeAction(in Point point, TouchAction action)
        {
        }

        protected virtual void AfterAction(in Point point, TouchAction action)
        {
        }

        public delegate bool ActionHandler(Component cmp, Point p);

        public ActionHandler OnPress { get; set; }
        public ActionHandler OnMove { get; set; }
        public ActionHandler OnRelease { get; set; }

        public bool IsPointInside(in Point p) => p.X >= Point.X && p.Y >= Point.Y && p.X < Point.X + Bounds.W && p.Y < Point.Y + Bounds.H;

        public bool HandleAction(in Point point, TouchAction action)
        {
            if (!IsPointInside(point))
            {
                //Console.WriteLine($"{point} NOT in ({Point} .. {Point + Bounds})");
                return false;
            }

            // Make sure the most descendent child gets a chance to react first:
            Point relPoint = point - Point;
            foreach (var child in Children)
            {
                if (child.HandleAction(relPoint, action)) return true;
            }

            // Determine which handler function to invoke:
            ActionHandler fn = null;
            switch (action)
            {
                case TouchAction.Pressed: fn = OnPress; break;
                case TouchAction.Moved: fn = OnMove; break;
                case TouchAction.Released: fn = OnRelease; break;
            }

            BeforeAction(point, action);
            bool handled = fn?.Invoke(this, point) ?? false;
            AfterAction(point, action);

            return handled;
        }
    }

    public enum Dock
    {
        Fill,
        Left,
        Top,
        Right,
        Bottom
    }
}
