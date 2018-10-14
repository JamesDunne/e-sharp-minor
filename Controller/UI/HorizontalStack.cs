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

        protected override void RenderSelf()
        {
            foreach (var child in Children)
            {
                child.Render();
            }
        }

        protected override void CalculateChildrenLayout(Point point, Bounds bounds, List<Component> fillChildren)
        {
            float step = bounds.W / (float)fillChildren.Count;

            // Arrange children horizontally:
            foreach (var child in fillChildren)
            {
                child.ComputedPoint = point;
                child.ComputedBounds = new Bounds(step, bounds.H);
                child.CalculateLayout();
                point.X += step;
            }
        }
    }
}
