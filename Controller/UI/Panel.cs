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

        protected override void RenderSelf()
        {
            // No render.
        }
    }
}