using System;
using System.Linq;
using System.Collections.Generic;

namespace EMinor.UI
{
    public class HorizontalStack : Component
    {
        public HorizontalStack(IPlatform platform)
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

            float step = this.Bounds.W / (float)Children.Count;

            // Arrange children horizontally:
            Point p = Point.Zero;
            foreach (var child in Children)
            {
                child.ComputedPoint = p;
                child.ComputedBounds = new Bounds(step, Bounds.H);
                child.CalculateChildrenLayout();
                p.X += step;
            }
        }
    }
}
