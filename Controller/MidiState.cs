using System;
using System.Collections.Generic;

namespace EMinor
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

        class ChannelBatchState
        {
            public bool ProgramChanged;
            public int ProgramValue;
            public Dictionary<int, int> ControllersModified;
        }

        private bool batchMode = false;
        private Dictionary<int, ChannelBatchState> batchChannels;

        /// <summary>
        /// Start a batch mode to queue up MIDI changes per channel.
        /// </summary>
        public void StartBatch()
        {
            batchMode = true;
            batchChannels = new Dictionary<int, ChannelBatchState>();
        }

        /// <summary>
        /// Ends batch mode and sends out latest MIDI program and controller changes per channel.
        /// </summary>
        public void EndBatch()
        {
            // Disable batch mode:
            batchMode = false;

            // Send out minimal MIDI updates comparing against pre-batch state:
            foreach (var channel in batchChannels.Keys)
            {
                var channelState = batchChannels[channel];
                if (channelState.ProgramChanged)
                {
                    // Change program:
                    this.SetProgram(channel, channelState.ProgramValue);
                }

                // Update all controller values for this channel:
                foreach (var controller in channelState.ControllersModified.Keys)
                {
                    this.SetController(channel, controller, channelState.ControllersModified[controller]);
                }
            }
        }

        public void SetController(int channel, int controller, int value)
        {
            if (batchMode)
            {
                ChannelBatchState channelState;
                if (!batchChannels.TryGetValue(channel, out channelState))
                {
                    channelState = new ChannelBatchState()
                    {
                        ControllersModified = new Dictionary<int, int>()
                    };
                    batchChannels.Add(channel, channelState);
                }

                // Remember the controller value:
                channelState.ControllersModified[controller] = value;
                return;
            }

            if (!Enabled || channels[channel].controllerValues[controller] != value)
            {
                midi.SetController(channel, controller, value);
                channels[channel].controllerValues[controller] = value;
            }
        }

        public void SetProgram(int channel, int program)
        {
            if (batchMode)
            {
                ChannelBatchState controllerSet;
                if (!batchChannels.TryGetValue(channel, out controllerSet))
                {
                    controllerSet = new ChannelBatchState();
                    batchChannels.Add(channel, controllerSet);
                }

                // Clear the state and indicate program changed:
                controllerSet.ControllersModified = new Dictionary<int, int>();
                controllerSet.ProgramChanged = true;
                controllerSet.ProgramValue = program;
                return;
            }

            if (!Enabled || channels[channel].programValue != program)
            {
                // Forget all sent data now that we're switching programs:
                channels[channel].Reset();
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
