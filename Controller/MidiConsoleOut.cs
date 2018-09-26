using System;
namespace EMinor
{
	public class MidiConsoleOut : IMIDI
    {
        public MidiConsoleOut()
        {
        }

        public void Dispose()
        {
        }

        public void SetController(int channel, int controller, int value)
        {
            Console.WriteLine("MIDI: {0:X2} {1:X2} {2:X2}", 0xB0 | (channel & 15), controller, value);
        }

        public void SetProgram(int channel, int program)
        {
            Console.WriteLine("MIDI: {0:X2} {1:X2}", 0xC0 | (channel & 15), program);
        }
    }
}
