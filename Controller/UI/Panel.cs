using System;
using Shapes;

namespace EMinor.UI
{
    public class Panel : Component
    {
        public Panel(IPlatform platform, Point point, Bounds bounds)
            : base(platform, point, bounds)
        {
        }

        public override void Render()
        {
        }
    }
}