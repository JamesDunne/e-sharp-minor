using System;
using System.IO;

namespace EMinor
{
    public class MidiAlsaOut : IMIDI, IDisposable
    {
        private readonly string devicePath;
        private readonly FileStream device;
        private readonly byte[] cmdbuf;

        public MidiAlsaOut(string devicePath = "/dev/snd/midiC1D0")
        {
            this.devicePath = devicePath;
            this.device = System.IO.File.OpenWrite(this.devicePath);
            this.cmdbuf = new byte[3];
        }

        public void Dispose()
        {
            this.device.Dispose();
        }

        public void SetController(int channel, int controller, int value)
        {
            this.cmdbuf[0] = (byte)(0xB0 | (channel & 15));
            this.cmdbuf[1] = (byte)controller;
            this.cmdbuf[2] = (byte)value;
            this.device.Write(this.cmdbuf, 0, 3);
        }

        public void SetProgram(int channel, int program)
        {
            this.cmdbuf[0] = (byte)(0xC0 | (channel & 15));
            this.cmdbuf[1] = (byte)program;
            this.device.Write(this.cmdbuf, 0, 2);
        }
    }
}
