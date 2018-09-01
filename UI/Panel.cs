using System;
using e_sharp_minor;

namespace UI
{
    public class Panel : Component
    {
        public Panel(IPlatform platform, float x, float y, float width, float height)
            : base(platform, x, y, width, height)
        {
        }

        public override void Render()
        {
        }
    }
}