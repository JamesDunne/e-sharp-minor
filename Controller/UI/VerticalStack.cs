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

        protected override void RenderSelf()
        {
            foreach (var child in Children)
            {
                child.Render();
            }
        }

        protected override void CalculateChildrenLayout(Point point, Bounds bounds, List<Component> fillChildren)
        {
            float step = bounds.H / (float)fillChildren.Count;

            // Arrange children vertically:
            foreach (var child in fillChildren)
            {
                child.ComputedPoint = point;
                child.ComputedBounds = new Bounds(bounds.W, step);
                child.CalculateLayout();
                point.Y += step;
            }
        }
    }
}
