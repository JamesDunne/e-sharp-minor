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

        public void StartBatch()
        {

        }

        public void EndBatch()
        {

        }

        public void SetController(int channel, int controller, int value)
        {
            Console.Out.WriteLineAsync($"MIDI: {0xB0 | (channel & 15):X02} {controller:X02} {value:X02}");
        }

        public void SetProgram(int channel, int program)
        {
            Console.Out.WriteLineAsync($"MIDI: {0xC0 | (channel & 15):X02} {program:X02}");
        }
    }
}
