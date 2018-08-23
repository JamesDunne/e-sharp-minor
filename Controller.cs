using System;
using System.Collections.Generic;
using System.Linq;
using e_sharp_minor.V6;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static System.IO.File;

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
                    throw new Exception("Midi program must define at least one amp!");
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
                        for (int i = 0; i < song.Amps.Count; i++)
                        {
                            song.Amps[i].AmpDefinition = midiProgram.Amps[i];
                        }
                    }

                    foreach (var scene in song.SceneDescriptors)
                    {
                        for (int i = 0; i < scene.Amps.Count; i++)
                        {
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

        public void ActivateSong(Song newSong)
        {
            int scene = 0;

            if (currentSong != null)
            {
                // TODO
            }

            midi.SetProgram(channel, newSong.MidiProgram.ProgramNumber);

            // TODO
            //midi.SetController(channel, );
            //newSong.MidiProgram.Amps[0];
            //newSong.Amps[0];
            //newSong.SceneDescriptors[scene].Amps[0].Tone;

            this.currentSong = newSong;
        }
    }

    namespace V6
    {
        public class AllPrograms
        {
            public List<MidiProgram> MidiPrograms { get; set; }
        }

        public class MidiProgram
        {
            [YamlMember(Alias = "midi", ApplyNamingConventions = false)]
            public int ProgramNumber { get; set; }

            public List<AmpDefinition> Amps { get; set; }

            public List<Song> Songs { get; set; }
        }

        public class Song
        {
            [YamlIgnore]
            public MidiProgram MidiProgram { get; set; }

            public string Name { get; set; }
            public int Tempo { get; set; }

            public List<AmpOverrides> Amps { get; set; }

            [YamlMember(Alias = "scenes", ApplyNamingConventions = false)]
            public List<SceneDescriptor> SceneDescriptors { get; set; }
        }

        public class AmpDefinition
        {
            [YamlIgnore]
            public MidiProgram MidiProgram { get; set; }

            public string Name { get; set; }

            // Available FX blocks for this amp in this MIDI program, including amp, cab, gate, etc.:
            public Dictionary<string, FXBlockDefinition> Blocks { get; set; }

            // MIDI CC of external controller that is mapped to gain:
            [YamlMember(Alias = "GainControllerCc")]
            public int? GainControllerCC { get; set; }
            // MIDI CC of external controller that is mapped to volume:
            [YamlMember(Alias = "VolumeControllerCc")]
            public int? VolumeControllerCC { get; set; }

            // Available general tones for this amp and their block settings, e.g. clean, dirty, acoustic:
            public Dictionary<string, ToneDefinition> Tones { get; set; }
        }

        public class FXBlockDefinition
        {
            [YamlMember(Alias = "EnabledSwitchCc")]
            public int EnabledSwitchCC { get; set; }
            [YamlMember(Alias = "XySwitchCc")]
            public int? XYSwitchCC { get; set; }
        }

        public enum XYSwitch
        {
            X,
            Y
        }

        public class FXBlock
        {
            public bool? On { get; set; }
            [YamlMember(Alias = "Xy")]
            public XYSwitch? XY { get; set; }
        }

        public class ToneDefinition
        {
            [YamlIgnore]
            public AmpDefinition AmpDefinition { get; set; }

            public int Gain { get; set; }
            public double Level { get; set; }

            public Dictionary<string, FXBlock> Blocks { get; set; }
        }

        public class FXBlockOverride
        {
            public bool? On { get; set; }
            [YamlMember(Alias = "Xy")]
            public XYSwitch? XY { get; set; }
        }

        public class ToneOverride
        {
            [YamlIgnore]
            public ToneDefinition ToneDefinition { get; set; }

            public int? Gain { get; set; }
            public double? Level { get; set; }

            public Dictionary<string, FXBlockOverride> Blocks { get; set; }
        }

        public class AmpOverrides
        {
            [YamlIgnore]
            public AmpDefinition AmpDefinition { get; set; }

            public Dictionary<string, ToneOverride> Tones { get; set; }
        }

        public class ToneSelection : ToneOverride
        {
            public string Tone { get; set; }
        }

        public class SceneDescriptor
        {
            public string Name { get; set; }
            public List<ToneSelection> Amps { get; set; }
        }
    }
}
