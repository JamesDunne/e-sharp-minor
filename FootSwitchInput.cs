using System;
using System.IO;
using System.Runtime.InteropServices;

namespace e_sharp_minor
{
    public class FootSwitchInput : IDisposable
    {
        private readonly string devicePath;
        private readonly int fd;
        private readonly byte[] eventBytes;

        private static int sizeOfInputEvent = Marshal.SizeOf<InputEvent>();

        public FootSwitchInput(string devicePath = "/dev/input/by-id/usb-413d_2107-event-mouse")
        {
            this.devicePath = devicePath;
            this.fd = open(devicePath, OpenFlags.ReadOnly | OpenFlags.NonBlock);
            this.eventBytes = new byte[sizeOfInputEvent];
        }

        public void Dispose()
        {
            close(fd);
        }

        const ushort EV_SYN = 0x00;
        const ushort EV_KEY = 0x01;
        //const ushort EV_ABS = 0x03;

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
            pollEvents();
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

        private unsafe void pollEvents()
        {
            fixed (byte* evBytePtr = eventBytes)
            {
                // Read input events until no more are left:
                while (read(fd, evBytePtr, (UIntPtr)sizeOfInputEvent) != IntPtr.Zero)
                {
                    ushort type;
                    ushort code;
                    int value;

                    // Copy values out of byte[] directly:
                    InputEvent* ev = (InputEvent*)evBytePtr;
                    type = ev->Type;

                    // We're done reading on an EV_SYN event type:
                    if (type == EV_SYN) break;

                    code = ev->Code;
                    value = ev->Value;

                    // Ignore anything but an EV_KEY event:
                    if (type != EV_KEY) continue;

                    // Figure out which button was pressed or released:
                    Console.WriteLine("{0} {1} {2}", type, code, value);
                    FootSwitchAction(new Action
                    {
                        FootSwitch = (code == 0x1E) ? FootSwitch.Left : (code == 0x30) ? FootSwitch.Right : FootSwitch.None,
                        WhatAction = translateValue(value)
                    });
                }
            }
        }

        #region Linux evdev structs

        const string lib = "libc";

        [DllImport(lib)]
        private static extern int ioctl(int d, EvdevIoctl request, [Out] IntPtr data);

        [DllImport(lib)]
        private static extern int ioctl(int d, uint request, [Out] IntPtr data);

        [DllImport(lib)]
        private static extern int open([MarshalAs(UnmanagedType.LPStr)]string pathname, OpenFlags flags);

        [DllImport(lib)]
        private static extern int close(int fd);

        [DllImport(lib)]
        unsafe private static extern IntPtr read(int fd, void* buffer, UIntPtr count);

        private enum EvdevIoctl : uint
        {
            Id = (2u << 30) | ((byte)'E' << 8) | (0x02u << 0) | (8u << 16), //EVIOCGID = _IOR('E', 0x02, struct input_id)
            Name128 = (2u << 30) | ((byte)'E' << 8) | (0x06u << 0) | (128u << 16), //EVIOCGNAME(len) = _IOC(_IOC_READ, 'E', 0x06, len)
        }

        [Flags]
        private enum OpenFlags
        {
            ReadOnly = 0x0000,
            WriteOnly = 0x0001,
            ReadWrite = 0x0002,
            NonBlock = 0x0800,
            CloseOnExec = 0x0080000
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct InputEvent
        {
            public TimeVal Time;
            public ushort Type;
            public ushort Code;
            public int Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TimeVal
        {
            public IntPtr Seconds;
            public IntPtr MicroSeconds;
        }

        #endregion
    }
}
