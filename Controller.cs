using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using e_sharp_minor.V6;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static System.IO.File;
using static System.Math;

namespace e_sharp_minor
{
    public class Controller
    {
        private readonly MidiState midi;
        private readonly int channel;

        private AllPrograms programs;
        private List<Song> songsSorted;

        private Song currentSong;

        public Controller(IMIDI midi, int channel)
        {
            // Wrap the MIDI output device in a state-tracker:
            this.midi = (midi is MidiState) ? (MidiState)midi : new MidiState(midi);
            this.channel = channel;
            this.currentSong = null;
        }

        public void LoadData()
        {
            var de = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var tr = OpenText("all-programs-v6.yml"))
                programs = de.Deserialize<V6.AllPrograms>(tr);

            // Set back-references since we can't really deserialize these:
            foreach (var midiProgram in programs.MidiPrograms)
            {
                if (midiProgram.Amps.Count == 0)
                {
                    throw new Exception("MIDI program must define at least one amp!");
                }

                foreach (var ampDefinition in midiProgram.Amps)
                {
                    ampDefinition.MidiProgram = midiProgram;

                    foreach (var tone in ampDefinition.Tones.Values)
                    {
                        tone.AmpDefinition = ampDefinition;
                    }
                }

                foreach (var song in midiProgram.Songs)
                {
                    song.MidiProgram = midiProgram;
                    if (song.Amps != null)
                    {
                        if (song.Amps.Count != midiProgram.Amps.Count)
                        {
                            throw new Exception(String.Format("Song '{0}' amp count must match MIDI program amp count!", song.Name));
                        }
                        for (int i = 0; i < song.Amps.Count; i++)
                        {
                            song.Amps[i].AmpDefinition = midiProgram.Amps[i];
                            foreach (var toneKey in song.Amps[i].Tones.Keys)
                            {
                                if (!midiProgram.Amps[i].Tones.ContainsKey(toneKey))
                                {
                                    throw new Exception(String.Format("Song '{0}' amp {1} tone '{2}' must exist in MIDI program amp definition!", song.Name, i + 1, toneKey));
                                }
                            }
                        }
                    }

                    foreach (var scene in song.SceneDescriptors)
                    {
                        if (scene.Amps.Count != midiProgram.Amps.Count)
                        {
                            throw new Exception(String.Format("Song '{0}' scene '{1}' amp count must match MIDI program amp count!", song.Name, scene.Name));
                        }
                        for (int i = 0; i < scene.Amps.Count; i++)
                        {
                            if (!midiProgram.Amps[i].Tones.ContainsKey(scene.Amps[i].Tone))
                            {
                                throw new Exception(String.Format("Song '{0}' scene '{1}' amp {2} tone '{3}' must exist in MIDI program amp definition!", song.Name, scene.Name, i + 1, scene.Amps[i].Tone));
                            }
                            scene.Amps[i].ToneDefinition = midiProgram.Amps[i].Tones[scene.Amps[i].Tone];
                        }
                    }
                }
            }

            // Sort songs alphabetically across all MIDI programs:
            songsSorted = programs
                            .MidiPrograms
                            .SelectMany(m => m.Songs)
                            .OrderBy(s => s.Name)
                            .ToList();

#if true
            foreach (var song in songsSorted)
            {
                Console.WriteLine("{0}", song.Name);
            }
#else
            foreach (var midiProgram in programs.MidiPrograms)
            {
                Console.WriteLine("midi: {0}", midiProgram.ProgramNumber);
                foreach (var song in midiProgram.Songs) {
                    Console.WriteLine("  song: {0}", song.Name);
                }
            }
#endif

            // Activate the first song:
            ActivateSong(songsSorted[0]);
        }

        int DBtoMIDI(double db)
        {
            db = db - 6.0;
            double p = Pow(10.0, (db / 20.0));
            double plog = Log10(p * 14.5 + 1.0) / Log10(15.5);
            plog *= 127.0;
            return (int)(Round(plog));
        }

        public void ActivateSong(Song newSong)
        {
            Console.WriteLine("Activate song '{0}'", newSong.Name);

            int scene = 0;

            if (currentSong != null)
            {
                // TODO: do we need to disable any blocks that are not in the new song?
            }

            Console.WriteLine("Change MIDI program {0}", newSong.MidiProgram.ProgramNumber);
            midi.SetProgram(channel, newSong.MidiProgram.ProgramNumber);

            for (int i = 0; i < newSong.MidiProgram.Amps.Count; i++)
            {
                SceneDescriptor sceneDescriptor = newSong.SceneDescriptors[scene];
                ToneSelection toneSelection = sceneDescriptor.Amps[i];
                ToneDefinition toneDefinition = toneSelection.ToneDefinition;
                AmpDefinition ampDefinition = toneDefinition.AmpDefinition;

                // Figure out the song-specific override of tone:
                ToneOverride toneOverride = null;
                if (newSong.Amps != null)
                {
                    toneOverride = newSong.Amps[i].Tones[toneSelection.Tone];
                }
                else
                {
                    toneOverride = new ToneOverride
                    {
                        Blocks = new Dictionary<string, FXBlockOverride>()
                    };
                }

                // Set all the controller values for the selected tone:
                foreach (var pair in toneDefinition.Blocks)
                {
                    FXBlock blockDefault = pair.Value;
                    string blockName = pair.Key;
                    FXBlockDefinition blockDefinition = ampDefinition.Blocks[blockName];

                    int enabledCC = blockDefinition.EnabledSwitchCC;
                    int? xySwitchCC = blockDefinition.XYSwitchCC;

                    FXBlockOverride songBlockOverride = null;
                    if (toneOverride != null)
                    {
                        songBlockOverride = toneOverride.Blocks.GetValueOrDefault(blockName);
                    }
                    if (songBlockOverride == null)
                    {
                        songBlockOverride = new FXBlockOverride();
                    }

                    // Follow inheritance chain to determine enabled and X/Y switch values:
                    Dictionary<string, FXBlockOverride> toneSelectionBlocks = toneSelection.Blocks ?? new Dictionary<string, FXBlockOverride>();
                    var sceneBlockOverride = toneSelectionBlocks.GetValueOrDefault(blockName) ?? new FXBlockOverride();

                    var enabled = sceneBlockOverride.On ?? songBlockOverride.On ?? blockDefault.On;
                    var xy = sceneBlockOverride.XY ?? songBlockOverride.XY ?? blockDefault.XY;

                    if (enabled.HasValue)
                    {
                        Console.WriteLine("Amp[{0}]: {1} = {2}", i+1, blockName, enabled.Value ? "on" : "off");
                        midi.SetController(channel, enabledCC, enabled.Value ? 0x7F : 0x00);
                    }
                    if (xy.HasValue && xySwitchCC.HasValue)
                    {
                        Console.WriteLine("Amp[{0}]: {1} to {2}", i + 1, blockName, enabled.Value ? "X" : "Y");
                        midi.SetController(channel, xySwitchCC.Value, xy.Value == XYSwitch.X ? 0x7F : 0x00);
                    }
                }

                // Set the gain and volume:
                var gain = toneSelection.Gain ?? toneOverride.Gain ?? toneDefinition.Gain;
                var volume = toneSelection.Volume ?? toneOverride.Volume ?? toneDefinition.Volume;

                // Convert volume to MIDI value:
                var volumeMIDI = DBtoMIDI(volume);

                Console.WriteLine("Amp[{0}]: gain   (CC {1:X2}h) to {2:X2}h", i + 1, toneDefinition.AmpDefinition.GainControllerCC, gain);
                midi.SetController(channel, toneDefinition.AmpDefinition.GainControllerCC, gain);
                Console.WriteLine("Amp[{0}]: volume (CC {1:X2}h) to {2:X2}h ({3} dB)", i + 1, toneDefinition.AmpDefinition.VolumeControllerCC, volumeMIDI, volume);
                midi.SetController(channel, toneDefinition.AmpDefinition.VolumeControllerCC, volumeMIDI);
            }

            this.currentSong = newSong;
        }
    }
}
