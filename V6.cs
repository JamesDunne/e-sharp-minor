using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace e_sharp_minor
{
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

        public class SceneDescriptor
        {
            public string Name { get; set; }
            public List<ToneSelection> Amps { get; set; }
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
            public int GainControllerCC { get; set; }
            // MIDI CC of external controller that is mapped to volume:
            [YamlMember(Alias = "VolumeControllerCc")]
            public int VolumeControllerCC { get; set; }

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
            public double Volume { get; set; }

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
            public double? Volume { get; set; }

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
    }
}
