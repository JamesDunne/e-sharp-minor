using System;
using System.IO;
using System.Runtime.InteropServices;

namespace e_sharp_minor
{
    public class FootSwitchInput : IDisposable
    {
        private readonly LinuxEventDevice evdev;

        public FootSwitchInput(string devicePath = "/dev/input/by-id/usb-413d_2107-event-mouse")
        {
            this.evdev = new LinuxEventDevice(devicePath);
            this.evdev.EventListener += (events) =>
            {
                foreach (var ev in events)
                {
                    if (ev.Type != LinuxEventDevice.EV_KEY) continue;

                    // Fire footswitch event:
                    FootSwitchAction(new Action
                    {
                        FootSwitch = (ev.Code == 0x1E) ? FootSwitch.Left : (ev.Code == 0x30) ? FootSwitch.Right : FootSwitch.None,
                        WhatAction = translateValue(ev.Value)
                    });
                }
            };
        }

        public void Dispose()
        {
            this.evdev.Dispose();
        }

        public enum ActionKind
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

        public struct Action
        {
            public FootSwitch FootSwitch;
            public ActionKind WhatAction;
        }

        public delegate void FootSwitchActionEvent(Action action);

        public event FootSwitchActionEvent FootSwitchAction;

        public void PollFootSwitches()
        {
            evdev.PollEvents();
        }

        private ActionKind translateValue(int value)
        {
            switch (value)
            {
                case 0: return ActionKind.Released;
                case 1: return ActionKind.Pressed;
                case 2: return ActionKind.AutoRepeat;
                default: return ActionKind.Released;
            }
        }
    }
}
