﻿using System;
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

        protected readonly List<Component> children;
        private Point? _computedPoint;
        private Bounds? _computedBounds;

        public bool LayoutCalculated;
        public bool ShapeCreated;

        protected Component(IPlatform platform)
        {
            this.platform = platform;
            this.vg = platform.VG;
            this.children = new List<Component>();
            Point = EMinor.Point.Zero;
            Bounds = platform.Bounds;
        }

        public virtual void Dispose()
        {
            this.children.ForEach(c => c.Dispose());
        }

        public virtual void SetParent(Component component)
        {
            this.Parent = component;
            Bounds = Parent?.Bounds ?? platform.Bounds;
            RootOffset = Point + (Parent?.RootOffset ?? EMinor.Point.Zero);
        }

        public virtual IList<Component> Children
        {
            get { return this.children; }
            set
            {
                this.children.ForEach(c => c.Dispose());
                this.children.Clear();
                this.children.AddRange(value);
                this.children.ForEach(c => c.SetParent(this));
                this.LayoutCalculated = false;
            }
        }
        public Component Parent { get; private set; }

        public Dock Dock { get; set; }

        public float? Width { get; set; }
        public float? Height { get; set; }

        public Point? ComputedPoint
        {
            get => _computedPoint;
            set
            {
                _computedPoint = value;
                Point = _computedPoint ?? EMinor.Point.Zero;
                RootOffset = Point + (Parent?.RootOffset ?? EMinor.Point.Zero);
            }
        }
        public Bounds? ComputedBounds
        {
            get => _computedBounds;
            set
            {
                _computedBounds = value;
                Bounds = _computedBounds ?? Parent?.Bounds ?? platform.Bounds;
                ShapeCreated = false;
            }
        }

        public Point Point; // => ComputedPoint ?? EMinor.Point.Zero;
        public Bounds Bounds; // => ComputedBounds ?? Parent?.Bounds ?? platform.Bounds;

        public Point RootOffset; // => Point + (Parent?.RootOffset ?? EMinor.Point.Zero);

        public Padding Padding { get; set; }

        protected virtual void CalculateChildrenLayout(Point point, Bounds bounds, List<Component> fillChildren)
        {
            // Naive fill implementation that merely overlaps all children:
            foreach (var child in fillChildren)
            {
                child.ComputedPoint = point;
                child.ComputedBounds = bounds;
                if (!child.LayoutCalculated)
                {
                    child.CalculateLayout();
                }
            }
        }

        public void UpdateShape()
        {
            CreateShape();
            ShapeCreated = true;
        }

        protected virtual void CreateShape()
        {
            // Override me to create OpenVG paths for the component.
        }

        public void CalculateLayout()
        {
            LayoutCalculated = true;

            if (!ShapeCreated)
            {
                UpdateShape();
            }

            Point point = new Point(Padding.Left, Padding.Bottom);
            Bounds bounds = Bounds - new Bounds(Padding.Left + Padding.Right, Padding.Bottom + Padding.Top);
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

                if (!child.LayoutCalculated)
                {
                    child.CalculateLayout();
                }
            }

            // Fill in remaining children:
            if (fillChildren.Count > 0)
            {
                CalculateChildrenLayout(point, bounds, fillChildren);
            }
        }

        public virtual unsafe void Render()
        {
            if (!LayoutCalculated)
            {
                CalculateLayout();
            }

            //float* m = stackalloc float[9];

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

        public virtual ActionHandler OnPress { get; set; }
        public virtual ActionHandler OnMove { get; set; }
        public virtual ActionHandler OnRelease { get; set; }

        public bool IsPointInside(in Point p) => p.X >= Point.X && p.Y >= Point.Y && p.X < Point.X + Bounds.W && p.Y < Point.Y + Bounds.H;

        public virtual bool IsPressable => (OnPress != null || OnRelease != null);

        public Component FindPressableComponent(in Point point)
        {
            if (!IsPointInside(point))
            {
                return null;
            }

            // Find the most descendent child still containing the point:
            Point relPoint = point - Point;
            foreach (var child in Children)
            {
                var selected = child.FindPressableComponent(relPoint);
                if (selected != null)
                {
                    return selected;
                }
            }

            // Only report this component if IsPressable:
            return IsPressable ? this : null;
        }

        public virtual bool HandleAction(in Point point, TouchAction action)
        {
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

        protected Point TranslateAlignment(HAlign hAlign, VAlign vAlign, Bounds inner)
        {
            float tx = 0f;
            switch (hAlign)
            {
                case HAlign.Left:
                    tx = 0f;
                    break;
                case HAlign.Right:
                    tx = Bounds.W - inner.W;
                    break;
                case HAlign.Center:
                    tx = (Bounds.W * 0.5f) - (inner.W * 0.5f);
                    break;
            }

            float ty = 0f;
            switch (vAlign)
            {
                case VAlign.Bottom:
                    break;
                case VAlign.Top:
                    ty = Bounds.H - inner.H;
                    break;
                case VAlign.Middle:
                    ty = (Bounds.H * 0.5f) - (inner.H * 0.5f);
                    break;
            }

            return new Point(tx, ty);
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

    public enum VAlign
    {
        Bottom,
        Middle,
        Top
    }

    public enum HAlign
    {
        Left,
        Center,
        Right
    }
}
