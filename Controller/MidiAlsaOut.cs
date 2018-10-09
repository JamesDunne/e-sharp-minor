using System;
using System.IO;

namespace EMinor
{
    public class MidiAlsaOut : IMIDI, IDisposable
    {
        private readonly string devicePath;
        private readonly int fd;

        public MidiAlsaOut(string devicePath = "/dev/snd/midiC1D0")
        {
            this.devicePath = devicePath;
            this.fd = LinuxEventDevice.open(this.devicePath, LinuxEventDevice.OpenFlags.WriteOnly | LinuxEventDevice.OpenFlags.NonBlock);
        }

        public void Dispose()
        {
            LinuxEventDevice.close(fd);
        }

        public void SetController(int channel, int controller, int value)
        {
            var cmdbuf = new byte[3];
            cmdbuf[0] = (byte)(0xB0 | (channel & 15));
            cmdbuf[1] = (byte)controller;
            cmdbuf[2] = (byte)value;
            unsafe
            {
                fixed (byte* buf = cmdbuf)
                {
                    LinuxEventDevice.write(fd, buf, 3);
                }
            }
            //this.device.Write(cmdbuf, 0, 3);
            Console.WriteLine("MIDI: {0:X02} {1:X02} {2:X02}", 0xB0 | (channel & 15), controller, value);
        }

        public void SetProgram(int channel, int program)
        {
            var cmdbuf = new byte[2];
            cmdbuf[0] = (byte)(0xC0 | (channel & 15));
            cmdbuf[1] = (byte)program;
            unsafe
            {
                fixed (byte* buf = cmdbuf)
                {
                    LinuxEventDevice.write(fd, buf, 2);
                }
            }
            //this.device.Write(cmdbuf, 0, 2);
            Console.WriteLine("MIDI: {0:X02} {1:X02}", 0xC0 | (channel & 15), program);
        }
    }
}
