using System;
using System.Linq;
using System.Collections.Generic;

namespace EMinor.UI
{
    public class VerticalStack : Component
    {
        public VerticalStack(IPlatform platform)
            : base(platform)
        {
        }

        public override void RenderSelf()
        {
            foreach (var child in Children)
            {
                child.RenderSelf();
            }
        }

        public override void CalculateChildrenLayout()
        {
            if (Children.Count == 0) return;

            float step = this.Bounds.H / (float)Children.Count;

            // Arrange children vertically:
            Point p = Point.Zero;
            foreach (var child in Children)
            {
                child.ComputedPoint = p;
                child.ComputedBounds = new Bounds(Bounds.W, step);
                child.CalculateChildrenLayout();
                p.Y += step;
            }
        }
    }
}
