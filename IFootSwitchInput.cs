using System;

namespace e_sharp_minor
{
    public interface IFootSwitchInput : IDisposable
    {
        void PollEvents();

        event FootSwitchEventListener EventListener;
    }

    public enum FootSwitchAction
    {
        Released,
        Pressed,
        AutoRepeat
    }

    public enum FootSwitch
    {
        None,
        Left,
        Right
    }

    public struct FootSwitchEvent
    {
        public FootSwitch FootSwitch;
        public FootSwitchAction WhatAction;
    }

    public delegate void FootSwitchEventListener(FootSwitchEvent ev);
}
