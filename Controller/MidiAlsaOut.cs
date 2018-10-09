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
            Console.Out.WriteLineAsync($"MIDI: {0xB0 | (channel & 15):X02} {controller:X02} {value:X02}");
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
            Console.Out.WriteLineAsync($"MIDI: {0xC0 | (channel & 15):X02} {program:X02}");
        }
    }
}
