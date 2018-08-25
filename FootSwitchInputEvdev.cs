using System;
using System.IO;
using System.Runtime.InteropServices;

namespace e_sharp_minor
{
    public class FootSwitchInputEvdev : IFootSwitchInput
    {
        private readonly LinuxEventDevice evdev;

        public FootSwitchInputEvdev(string devicePath = "/dev/input/by-id/usb-413d_2107-event-mouse")
        {
            this.evdev = new LinuxEventDevice(devicePath);

            // Process a sequence of evdev events:
            this.evdev.EventListener += (events) =>
            {
                foreach (var ev in events)
                {
                    if (ev.Type != LinuxEventDevice.EV_KEY) continue;

                    // Fire footswitch event:
                    EventListener(new FootSwitchEvent
                    {
                        FootSwitch = (ev.Code == 0x1E) ? FootSwitch.Left : (ev.Code == 0x30) ? FootSwitch.Right : FootSwitch.None,
                        WhatAction = translateValue(ev.Value)
                    });
                }
            };
        }

        public event FootSwitchEventListener EventListener;

        public void Dispose()
        {
            this.evdev.Dispose();
        }

        public void PollEvents()
        {
            evdev.PollEvents();
        }

        private FootSwitchAction translateValue(int value)
        {
            switch (value)
            {
                case 0: return FootSwitchAction.Released;
                case 1: return FootSwitchAction.Pressed;
                case 2: return FootSwitchAction.AutoRepeat;
                default: return FootSwitchAction.Released;
            }
        }
    }
}
