using System;
using System.Threading;
using OpenVG;

namespace EMinor
{
    public interface IPlatform : IDisposable
    {
        IOpenVG VG { get; }

        IMIDI MIDI { get; }

        event InputEventDelegate InputEvent;

        int Width { get; }
        int Height { get; }

        float MaxX { get; }
        float MaxY { get; }

        Bounds Bounds { get; }

        int FramebufferWidth { get; }
        int FramebufferHeight { get; }

        void SwapBuffers();
        bool ShouldQuit();
        void WaitEvents();
        void PollEvents();

        void InitRenderThread();
        Thread NewRenderThread(ThreadStart threadStart);
    }

    public enum TouchAction
    {
        Released,
        Pressed,
        Moved
    }

    public struct TouchEvent
    {
        public Point Point;
        public TouchAction Action;
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
        public FootSwitchAction Action;
    }

    public struct InputEvent
    {
        public FootSwitchEvent? FootSwitchEvent;
        public TouchEvent? TouchEvent;
    }

    public delegate void InputEventDelegate(InputEvent @event);
}
