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
            Console.WriteLine("MIDI: {0:X02} {1:X02} {2:X02}", 0xB0 | (channel & 15), controller, value);
        }

        public void SetProgram(int channel, int program)
        {
            Console.WriteLine("MIDI: {0:X02} {1:X02}", 0xC0 | (channel & 15), program);
        }
    }
}
