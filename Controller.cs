using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenVG;
using Shapes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static System.IO.File;

namespace e_sharp_minor
{
    public class Controller
    {
        private V5.AllPrograms programs;

        public Controller()
        {
        }

        private V6.ToneSelection convertAmp(V5.Amp amp)
        {
            return new V6.ToneSelection
            {
                Tone = amp.Channel,
                Gain = amp.Gain == 0 ? (int?)null : amp.Gain,
                Level = amp.Level == 0 ? (double?)null : amp.Level,
                Blocks = amp.FX?.ToDictionary(
                    fx => fx,
                    fx => new V6.FXBlockOverride
                    {
                        On = true
                    }
                )
            };
        }

        private V6.AllPrograms convertV5toV6(V5.AllPrograms programs)
        {
            return new V6.AllPrograms
            {
                MidiPrograms = (
                    from g in (
                        from p in programs.Programs
                        group p by p.MidiProgram
                    )
                    select new V6.MidiProgram
                    {
                        ProgramNumber = g.Key,
                        Amps = new List<V6.AmpDefinition>
                        {
                            // MG:
                            new V6.AmpDefinition
                            {
                                Name = "MG",
                                Blocks = new Dictionary<string, V6.FXBlockDefinition>
                                {
                                    { "amp1", new V6.FXBlockDefinition { EnabledSwitchCC = 37, XYSwitchCC = 100 } },
                                    { "cab1", new V6.FXBlockDefinition { EnabledSwitchCC = 39, XYSwitchCC = 102 } },
                                    { "gate1", new V6.FXBlockDefinition { EnabledSwitchCC = 60 } },
                                    { "compressor1", new V6.FXBlockDefinition { EnabledSwitchCC = 43 } }
                                    // TODO: add fx_layout blocks
                                },
                                Tones = new Dictionary<string, V6.ToneDefinition>
                                {
                                    { "clean", new V6.ToneDefinition {
                                        Gain = 0x12,
                                        Level = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp1", new V6.FXBlock { On = true, XY = V6.XYSwitch.Y } },
                                            { "cab1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "gate1", new V6.FXBlock { On = false, XY = V6.XYSwitch.X } },
                                            { "compressor1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } },
                                    { "dirty", new V6.ToneDefinition {
                                        Gain = 0x40,
                                        Level = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "cab1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "gate1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "compressor1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } },
                                    { "acoustic", new V6.ToneDefinition {
                                        Gain = 0x12,
                                        Level = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp1", new V6.FXBlock { On = false, XY = V6.XYSwitch.Y } },
                                            { "cab1", new V6.FXBlock { On = true, XY = V6.XYSwitch.Y } },
                                            { "gate1", new V6.FXBlock { On = false, XY = V6.XYSwitch.X } },
                                            { "compressor1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } }
                                },
                                VolumeControllerCC = 16,
                                GainControllerCC = 18
                            },
                            // JD:
                            new V6.AmpDefinition
                            {
                                Name = "JD",
                                Blocks = new Dictionary<string, V6.FXBlockDefinition>
                                {
                                    { "amp2", new V6.FXBlockDefinition { EnabledSwitchCC = 38, XYSwitchCC = 101 } },
                                    { "cab2", new V6.FXBlockDefinition { EnabledSwitchCC = 40, XYSwitchCC = 103 } },
                                    { "gate2", new V6.FXBlockDefinition { EnabledSwitchCC = 61 } },
                                    { "compressor2", new V6.FXBlockDefinition { EnabledSwitchCC = 44 } }
                                },
                                Tones = new Dictionary<string, V6.ToneDefinition>
                                {
                                    { "clean", new V6.ToneDefinition {
                                        Gain = 0x12,
                                        Level = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp2", new V6.FXBlock { On = true, XY = V6.XYSwitch.Y } },
                                            { "cab2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "gate2", new V6.FXBlock { On = false, XY = V6.XYSwitch.X } },
                                            { "compressor2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } },
                                    { "dirty", new V6.ToneDefinition {
                                        Gain = 0x40,
                                        Level = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "cab2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "gate2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "compressor2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } },
                                    { "acoustic", new V6.ToneDefinition {
                                        Gain = 0x12,
                                        Level = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp2", new V6.FXBlock { On = false, XY = V6.XYSwitch.Y } },
                                            { "cab2", new V6.FXBlock { On = true, XY = V6.XYSwitch.Y } },
                                            { "gate2", new V6.FXBlock { On = false, XY = V6.XYSwitch.X } },
                                            { "compressor2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } }
                                },
                                VolumeControllerCC = 17,
                                GainControllerCC = 19
                            }
                        },
                        Songs = (
                            from p in g
                            select new V6.Song
                            {
                                Name = p.Name,
                                Tempo = p.Tempo,
                                Amps = new List<V6.AmpOverrides>
                                {
                                    new V6.AmpOverrides
                                    {
                                        Tones = new Dictionary<string, V6.ToneOverride>
                                        {
                                            {
                                                "dirty",
                                                new V6.ToneOverride
                                                {
                                                    Gain = p.Gain
                                                }
                                            }
                                        }
                                    }
                                },
                                SceneDescriptors = (
                                    from s in p.SceneDescriptors
                                    select new V6.SceneDescriptor
                                    {
                                        Name = s.Name,
                                        Amps = new List<V6.ToneSelection>
                                        {
                                            convertAmp(s.MG),
                                            convertAmp(s.JD)
                                        }
                                    }
                                ).ToList()
                            }
                        ).ToList()
                    }
                ).ToList()
            };
        }

        public void LoadData()
        {
            var de = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var tr = OpenText("all-programs-v5.yml"))
                programs = de.Deserialize<V5.AllPrograms>(tr);

            var newPrograms = convertV5toV6(programs);

            var se = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .EmitDefaults()
                .Build();

            using (var tw = CreateText("all-programs-v6.yml"))
                se.Serialize(tw, newPrograms);
        }
    }

    namespace V5
    {
        public class AllPrograms
        {
            public List<AmpDefault> Amp { get; set; }
            public List<Program> Programs { get; set; }
        }

        public class AmpDefault
        {
            [YamlMember(Alias = "fx_layout")]
            public List<string> FXLayout { get; set; } // `yaml:"fx_layout"`
            public int Gain { get; set; }          // `yaml:"gain"`     // amp gain (1-127), 0 means default
            public int GainLog { get; set; }       // `yaml:"gain_log"` // amp gain (1-127) in log scale, 0 means default
        }

        public class Program
        {
            public string Name { get; set; } // `yaml:"name"`
            [YamlMember(Alias = "midi", ApplyNamingConventions = false)]
            public int MidiProgram { get; set; } // `yaml:"midi"`
            public int Tempo { get; set; } // `yaml:"tempo"`
            public int Gain { get; set; } // `yaml:"gain"`     // amp gain (1-127), 0 means default
            public int GainLog { get; set; } // `yaml:"gain_log"` // amp gain (1-127) in log scale, 0 means default
            public List<AmpDefault> Amp { get; set; } // `yaml:"amp"`
            [YamlMember(Alias = "scenes", ApplyNamingConventions = false)]
            public List<SceneDescriptor> SceneDescriptors { get; set; } // `yaml:"scenes"`
        }

        public class SceneDescriptor
        {
            public string Name { get; set; }
            [YamlMember(Alias = "MG", ApplyNamingConventions = false)]
            public Amp MG { get; set; } // `yaml:"MG"`
            [YamlMember(Alias = "JD", ApplyNamingConventions = false)]
            public Amp JD { get; set; } // `yaml:"JD"`
        }

        public class Amp
        {
            public int Gain { get; set; } // `yaml:"gain"`     // amp gain (1-127), 0 means default
            public int GainLog { get; set; } // `yaml:"gain_log"` // amp gain (1-127) in log scale, 0 means default
            public string Channel { get; set; } // `yaml:"channel"`  // "clean" or "dirty"
            public double Level { get; set; } // `yaml:"level"`    // pre-delay volume in dB (-inf to +6dB)
            [YamlMember(Alias = "fx", ApplyNamingConventions = false)]
            public List<string> FX { get; set; } // `yaml:"fx,flow"`  // any combo of "delay", "pitch", or "chorus"
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
            public string Name { get; set; }
            public int Tempo { get; set; }

            public List<AmpOverrides> Amps { get; set; }

            [YamlMember(Alias = "scenes", ApplyNamingConventions = false)]
            public List<SceneDescriptor> SceneDescriptors { get; set; }
        }

        public class AmpDefinition
        {
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
            public bool On { get; set; }
            [YamlMember(Alias = "Xy")]
            public XYSwitch? XY { get; set; }
        }

        public class ToneDefinition
        {
            public int Gain { get; set; }
            public double Level { get; set; }

            public Dictionary<string, FXBlock> Blocks { get; set; }
        }

        public class FXBlockOverride
        {
            public bool? On { get; set; }
            [YamlMember(Alias = "Xy")]
            public bool? XY { get; set; }
        }

        public class ToneOverride
        {
            public int? Gain { get; set; }
            public double? Level { get; set; }

            public Dictionary<string, FXBlockOverride> Blocks { get; set; }
        }

        public class AmpOverrides
        {
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
