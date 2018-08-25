using System;
using System.IO;
using System.Runtime.InteropServices;

namespace e_sharp_minor
{
    public class FootSwitchInput : IDisposable
    {
        private readonly string devicePath;
        private readonly FileStream device;
        private readonly byte[] eventBytes;

        private static int sizeOfInputEvent = Marshal.SizeOf<InputEvent>();

        public FootSwitchInput(string devicePath = "/dev/input/by-id/usb-413d_2107-event-mouse")
        {
            this.devicePath = devicePath;
            this.device = System.IO.File.OpenRead(devicePath);
            this.eventBytes = new byte[sizeOfInputEvent];
        }

        public void Dispose()
        {
            this.device.Dispose();
        }

        const ushort EV_SYN = 0x00;
        const ushort EV_KEY = 0x01;
        //const ushort EV_ABS = 0x03;

        public bool PollFootSwitches(out bool? left, out bool? right)
        {
            bool wasRead = false;
            left = null;
            right = null;

            // Read input events until no more are left:
            while (this.device.Read(eventBytes, 0, sizeOfInputEvent) != 0)
            {
                ushort type;
                ushort code;
                int value;

                // Copy values out of byte[] directly:
                unsafe
                {
                    fixed (byte* evBytePtr = eventBytes)
                    {
                        InputEvent* ev = (InputEvent*)evBytePtr;
                        type = ev->Type;

                        // We're done reading on an EV_SYN event type:
                        if (type == EV_SYN) break;

                        code = ev->Code;
                        value = ev->Value;
                    }
                }

                // Ignore anything but an EV_KEY event:
                if (type != EV_KEY) continue;

                // Figure out which button was pressed or released:
                if (code == 0x1E) left = value != 0;
                if (code == 0x2E) right = value != 0;
                wasRead = true;
            }

            return wasRead;
        }

        #region Linux evdev structs

        [StructLayout(LayoutKind.Sequential)]
        struct InputEvent
        {
            public TimeVal Time;
            public ushort Type;
            public ushort Code;
            public int Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct TimeVal
        {
            public IntPtr Seconds;
            public IntPtr MicroSeconds;
        }

        #endregion
    }
}
