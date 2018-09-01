using System;

namespace e_sharp_minor
{
    public interface IFootSwitchInput : IDisposable
    {
        void PollEvents();

        event FootSwitchEventListener EventListener;
    }

    public delegate void FootSwitchEventListener(FootSwitchEvent ev);
}
