using System;
using System.Collections.Generic;
using System.IO;

namespace EMinor
{
    public class MidiAlsaOut : IMIDI, IDisposable
    {
        private readonly string devicePath;
        private readonly int fd;
        private bool batchMode;
        private List<byte> batch;

        public MidiAlsaOut(string devicePath = "/dev/snd/midiC1D0")
        {
            this.devicePath = devicePath;
            this.fd = LinuxEventDevice.open(this.devicePath, LinuxEventDevice.OpenFlags.WriteOnly | LinuxEventDevice.OpenFlags.NonBlock);
        }

        public void Dispose()
        {
            LinuxEventDevice.close(fd);
        }

        public void StartBatch()
        {
            this.batchMode = true;
            this.batch = new List<byte>();
        }

        public unsafe void EndBatch()
        {
            this.batchMode = false;
            fixed (byte* buf = this.batch.ToArray())
            {
                writeBytes(buf, this.batch.Count);
            }
        }

        private unsafe void writeBytes(byte* buf, int count)
        {
            LinuxEventDevice.write(fd, buf, (uint)count);
        }

        public unsafe void SetController(int channel, int controller, int value)
        {
            var cmdbuf = stackalloc byte[3];
            cmdbuf[0] = (byte)(0xB0 | (channel & 15));
            cmdbuf[1] = (byte)controller;
            cmdbuf[2] = (byte)value;

            if (batchMode)
            {
                this.batch.Add(cmdbuf[0]);
                this.batch.Add(cmdbuf[1]);
                this.batch.Add(cmdbuf[2]);
            }
            else
            {
                writeBytes(cmdbuf, 3);
            }

            Console.Out.WriteLineAsync($"MIDI: {0xB0 | (channel & 15):X02} {controller:X02} {value:X02}");
        }

        public unsafe void SetProgram(int channel, int program)
        {
            var cmdbuf = stackalloc byte[2];
            cmdbuf[0] = (byte)(0xC0 | (channel & 15));
            cmdbuf[1] = (byte)program;

            if (batchMode)
            {
                this.batch.Add(cmdbuf[0]);
                this.batch.Add(cmdbuf[1]);
            }
            else
            {
                writeBytes(cmdbuf, 2);
            }

            Console.Out.WriteLineAsync($"MIDI: {0xC0 | (channel & 15):X02} {program:X02}");
        }
    }
}
