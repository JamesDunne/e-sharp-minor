using System;
using Shapes;

namespace EMinor.UI
{
    public class Panel : Component
    {
        public Panel(IPlatform platform)
            : base(platform)
        {
        }

        public override void RenderSelf()
        {
            // No render.
        }

        public override void CalculateChildrenLayout()
        {
            foreach (var child in this.Children)
            {
                child.ComputedPoint = Point.Zero;
                child.ComputedBounds = Bounds;
                child.CalculateChildrenLayout();
            }
        }
    }
}