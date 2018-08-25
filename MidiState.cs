using System;
using System.Collections.Generic;

namespace e_sharp_minor
{
    // Prevents sending redundant MIDI commands.
    public class MidiState : IMIDI
    {
        private readonly IMIDI midi;
        private readonly ChannelState[] channels;

        public MidiState(IMIDI midi)
        {
            this.midi = midi;
            this.channels = new ChannelState[16];
            for (int i = 0; i < 16; i++)
            {
                this.channels[i] = new ChannelState();
            }
            this.Enabled = true;
        }

        /// <summary>
        /// Resets the state tracker to assume no data has been sent.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < 16; i++)
            {
                this.channels[i].Reset();
            }
        }

        /// <summary>
        /// Set to false to temporarily disable the state tracker and force sending MIDI commands 
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        public void SetController(int channel, int controller, int value)
        {
            if (!Enabled || channels[channel].controllerValues[controller] != value)
            {
                midi.SetController(channel, controller, value);
                channels[channel].controllerValues[controller] = value;
            }
        }

        public void SetProgram(int channel, int program)
        {
            if (!Enabled || channels[channel].programValue != program)
            {
                // Forget all sent data now that we're switching programs:
                Reset();
                midi.SetProgram(channel, program);
                channels[channel].programValue = program;
            }
        }

        public void Dispose()
        {
            this.midi.Dispose();
        }

        class ChannelState
        {
            public readonly int[] controllerValues;
            public int programValue;

            public ChannelState()
            {
                controllerValues = new int[256];
                Reset();
            }

            public void Reset()
            {
                programValue = -1;
                for (int i = 0; i < 256; i++)
                {
                    controllerValues[i] = -1;
                }
            }
        }
    }
}
