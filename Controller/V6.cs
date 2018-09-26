﻿using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace EMinor
{
    namespace V6
    {
        public class Setlist
        {
            public string Date { get; set; }
            public string Venue { get; set; }
            public bool Active { get; set; }
            public bool Print { get; set; }
            [YamlMember(Alias = "Songs")]
            public List<string> SongNames { get; set; }

            [YamlIgnore]
            public List<V6.Song> Songs { get; set; }
        }

        public class Setlists
        {
            public List<Setlist> Sets { get; set; }
        }

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
            public string ShortName { get; internal set; }
            public List<string> AlternateNames { get; internal set; }
            public string WhoStarts { get; internal set; }

            public int Tempo { get; set; }

            public List<AmpOverrides> Amps { get; set; }

            [YamlMember(Alias = "scenes", ApplyNamingConventions = false)]
            public List<SceneDescriptor> SceneDescriptors { get; set; }

            public bool MatchesName(string match)
            {
                return String.Compare(match, Name, true) == 0
                    || String.Compare(match, ShortName, true) == 0
                    || AlternateNames.Any(name => String.Compare(match, name, true) == 0);
            }
        }

        public class SceneDescriptor
        {
            [YamlIgnore]
            public Song Song { get; set; }
            [YamlIgnore]
            public int SceneNumber { get; set; }

            public string Name { get; set; }
            public List<AmpToneSelection> Amps { get; set; }
        }

        public class AmpDefinition
        {
            [YamlIgnore]
            public MidiProgram MidiProgram { get; set; }
            [YamlIgnore]
            public int AmpNumber { get; set; }

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
            public Dictionary<string, AmpToneDefinition> Tones { get; set; }
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

        public class AmpToneDefinition
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

        public class AmpToneOverride
        {
            [YamlIgnore]
            public AmpToneDefinition AmpToneDefinition { get; set; }

            public int? Gain { get; set; }
            public double? Volume { get; set; }

            public Dictionary<string, FXBlockOverride> Blocks { get; set; }
        }

        public class AmpOverrides
        {
            [YamlIgnore]
            public AmpDefinition AmpDefinition { get; set; }
            [YamlIgnore]
            public int AmpNumber { get; set; }

            public Dictionary<string, AmpToneOverride> Tones { get; set; }
        }

        public class AmpToneSelection : AmpToneOverride
        {
            [YamlIgnore]
            public int AmpNumber { get; set; }

            public string Tone { get; set; }
        }
    }
}