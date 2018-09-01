﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace e_sharp_minor
{
    public class LinuxEventDevice : IDisposable
    {
        private readonly string devicePath;
        private readonly int fd;
        private readonly byte[] eventBytes;

        private static int sizeOfInputEvent = Marshal.SizeOf<InputEvent>();

        public LinuxEventDevice(string devicePath)
        {
            this.devicePath = devicePath;
            this.fd = open(devicePath, OpenFlags.ReadOnly | OpenFlags.NonBlock);
            this.eventBytes = new byte[sizeOfInputEvent];
        }

        public void Dispose()
        {
            close(fd);
        }

        /// <summary>
        /// Implicitly convert to int file descriptor.
        /// </summary>
        /// <returns>The implicit.</returns>
        /// <param name="eventDevice">Event device.</param>
        public static implicit operator int(LinuxEventDevice eventDevice)
        {
            return eventDevice.fd;
        }

        public const ushort EV_SYN = 0x00;
        public const ushort EV_KEY = 0x01;
        public const ushort EV_ABS = 0x03;

        public struct Event
        {
            public ushort Type;
            public ushort Code;
            public int Value;
        }

        public delegate void EventListenerDelegate(List<Event> events);

        public event EventListenerDelegate EventListener;

        public void PollEvents()
        {
            pollEvents();
        }

        public static byte BitCount(ulong value)
        {
            ulong result = value - ((value >> 1) & 0x5555555555555555UL);
            result = (result & 0x3333333333333333UL) + ((result >> 2) & 0x3333333333333333UL);
            return (byte)(unchecked(((result + (result >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

        public static LinuxEventDevice[] WaitEvents(params LinuxEventDevice[] fds)
        {
            unsafe
            {
                // Build up fd_set for read:
                const int fd_size = 1024 / (8 * 8);
                ulong* fd_ptr = stackalloc ulong[fd_size];
                foreach (var dev in fds)
                {
                    int fd = dev;
                    fd_ptr[(fd / fd_size)] |= (1UL << (fd % fd_size));
                }

                // Await for read readiness:
                select(1, fd_ptr, null, null, IntPtr.Zero);

                // Count number of fds ready:
                int numReady = 0;
                for (int i = 0; i < fd_size; i++)
                {
                    numReady += BitCount(fd_ptr[i]);
                }

                // Build set of fds that are ready:
                var ready = new LinuxEventDevice[numReady];
                int n = 0;
                foreach (var dev in fds)
                {
                    int fd = dev;
                    ulong mask = (1UL << (fd % fd_size));
                    if ((fd_ptr[(fd / fd_size)] & mask) == mask)
                    {
                        ready[n] = dev;
                        n++;
                    }
                }

                return ready;
            }
        }

        private unsafe void pollEvents()
        {
            List<Event> events = null;

            fixed (byte* evBytePtr = eventBytes)
            {
                // Read input events until no more are left:
                while (read(fd, evBytePtr, (UIntPtr)sizeOfInputEvent) != IntPtr.Zero)
                {
                    Event ev;

                    // Copy values out of byte[] directly:
                    InputEvent* iev = (InputEvent*)evBytePtr;
                    ev.Type = iev->Type;

                    // We're done reading on an EV_SYN event type:
                    if (ev.Type == EV_SYN) break;

                    ev.Code = iev->Code;
                    ev.Value = iev->Value;

                    // Add the event to the collection:
                    Console.WriteLine("{0:X4} {1:X4} {2:X4}", ev.Type, ev.Code, ev.Value);
                    if (events == null) events = new List<Event>();
                    events.Add(ev);
                }
            }

            if (events != null)
            {
                // Fire event listener with the collection of events read in:
                EventListener(events);
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

        [DllImport(lib)]
        unsafe private static extern IntPtr select(int nfds, ulong* readfds, ulong* writefds, ulong* exceptfds, IntPtr timeout);

        private enum EvdevIoctl : uint
        {
            Id = (2u << 30) | ((byte)'E' << 8) | (0x02u << 0) | (8u << 16), //EVIOCGID = _IOR('E', 0x02, struct input_id)
            Name128 = (2u << 30) | ((byte)'E' << 8) | (0x06u << 0) | (128u << 16), //EVIOCGNAME(len) = _IOC(_IOC_READ, 'E', 0x06, len)
        }

#if false
        [StructLayout(LayoutKind.Sequential)]
        private struct fd_set
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 / (8 * 8))]
            public ulong[] fds_bits;
        }
#endif

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
