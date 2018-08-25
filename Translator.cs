using System;
using System.Collections.Generic;
using static System.IO.File;
using static System.Math;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;

namespace e_sharp_minor
{
    public class Translator
    {
        public Translator()
        {
        }

        private V5.SongNames songNames;
        private V5.AllPrograms v5programs;

        public void Translate()
        {
            var de = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var tr = OpenText("all-programs-v5.yml"))
                v5programs = de.Deserialize<V5.AllPrograms>(tr);

            using (var tr = OpenText("song-names.yml"))
                songNames = de.Deserialize<V5.SongNames>(tr);

            var v6programs = convertV5toV6();

            var se = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var tw = CreateText("all-programs-v6.yml"))
                se.Serialize(tw, v6programs);
        }

        private int logTaper(int b)
        {
            // 127 * (ln(x+1)^2) / (ln(127+1)^2)
            return (int)(127.0 * Pow(Math.Log((double)(b) + 1.0), 2) / Pow(Log(127.0 + 1.0), 2));
        }

        private V6.AmpToneSelection convertAmp(V5.Amp amp)
        {
            return new V6.AmpToneSelection
            {
                Tone = amp.Channel,
                Gain = amp.Gain == 0 ? (amp.GainLog == 0 ? (int?)null : logTaper(amp.GainLog)) : amp.Gain,
                Volume = amp.Level == 0 ? (double?)null : amp.Level,
                Blocks = amp.FX?.ToDictionary(
                    fx => fx,
                    fx => new V6.FXBlockOverride
                    {
                        On = true
                    }
                )
            };
        }

        private V6.AllPrograms convertV5toV6()
        {
            return new V6.AllPrograms
            {
                MidiPrograms = (
                    from g in (
                        from p in v5programs.Programs
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
                                    { "compressor1", new V6.FXBlockDefinition { EnabledSwitchCC = 43 } },
                                    { "pit1", new V6.FXBlockDefinition { EnabledSwitchCC = 77 } },
                                    { "rtr1", new V6.FXBlockDefinition { EnabledSwitchCC = 86 } },
                                    { "phr1", new V6.FXBlockDefinition { EnabledSwitchCC = 75 } },
                                    { "cho1", new V6.FXBlockDefinition { EnabledSwitchCC = 41 } },
                                    { "dly1", new V6.FXBlockDefinition { EnabledSwitchCC = 47 } }
                                },
                                Tones = new Dictionary<string, V6.AmpToneDefinition>
                                {
                                    { "clean", new V6.AmpToneDefinition {
                                        Gain = 0x12,
                                        Volume = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp1", new V6.FXBlock { On = true, XY = V6.XYSwitch.Y } },
                                            { "cab1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "gate1", new V6.FXBlock { On = false, XY = V6.XYSwitch.X } },
                                            { "compressor1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } },
                                    { "dirty", new V6.AmpToneDefinition {
                                        Gain = 0x40,
                                        Volume = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "cab1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "gate1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "compressor1", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } },
                                    { "acoustic", new V6.AmpToneDefinition {
                                        Gain = 0x12,
                                        Volume = 0,
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
                                    { "compressor2", new V6.FXBlockDefinition { EnabledSwitchCC = 44 } },
                                    { "pit2", new V6.FXBlockDefinition { EnabledSwitchCC = 78 } },
                                    { "rtr2", new V6.FXBlockDefinition { EnabledSwitchCC = 87 } },
                                    { "phr2", new V6.FXBlockDefinition { EnabledSwitchCC = 76 } },
                                    { "cho2", new V6.FXBlockDefinition { EnabledSwitchCC = 42 } },
                                    { "dly2", new V6.FXBlockDefinition { EnabledSwitchCC = 48 } }
                                },
                                Tones = new Dictionary<string, V6.AmpToneDefinition>
                                {
                                    { "clean", new V6.AmpToneDefinition {
                                        Gain = 0x12,
                                        Volume = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp2", new V6.FXBlock { On = true, XY = V6.XYSwitch.Y } },
                                            { "cab2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "gate2", new V6.FXBlock { On = false, XY = V6.XYSwitch.X } },
                                            { "compressor2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } },
                                    { "dirty", new V6.AmpToneDefinition {
                                        Gain = 0x40,
                                        Volume = 0,
                                        Blocks = new Dictionary<string, V6.FXBlock>
                                        {
                                            { "amp2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "cab2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "gate2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } },
                                            { "compressor2", new V6.FXBlock { On = true, XY = V6.XYSwitch.X } }
                                        }
                                    } },
                                    { "acoustic", new V6.AmpToneDefinition {
                                        Gain = 0x12,
                                        Volume = 0,
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
                            let alternateNames = (
                                from sn in songNames.Songs
                                where sn.Names.Any(name => String.Compare(p.Name, name, true) == 0) || String.Compare(p.Name, sn.ShortName, true) == 0
                                select sn
                            ).Single()
                            select new V6.Song
                            {
                                Name = p.Name,
                                ShortName = alternateNames.ShortName,
                                AlternateNames = alternateNames.Names,
                                WhoStarts = alternateNames.Starts,
                                Tempo = p.Tempo,
                                Amps = defaultAmps(p),
                                SceneDescriptors = (
                                    from s in p.SceneDescriptors
                                    select new V6.SceneDescriptor
                                    {
                                        Name = s.Name,
                                        Amps = new List<V6.AmpToneSelection>
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

        private List<V6.AmpOverrides> defaultAmps(V5.Program p)
        {
            int? g = p.Gain == 0 ? (p.GainLog == 0 ? (int?)null : logTaper(p.GainLog)) : p.Gain;
            if (!g.HasValue) return null;

            // These instances must be separate to avoid YAML serializer aliasing:
            return new List<V6.AmpOverrides>
            {
                new V6.AmpOverrides
                {
                    Tones = new Dictionary<string, V6.AmpToneOverride>
                    {
                        {
                            "dirty",
                            new V6.AmpToneOverride
                            {
                                Gain = g
                            }
                        }
                    }
                },
                new V6.AmpOverrides
                {
                    Tones = new Dictionary<string, V6.AmpToneOverride>
                    {
                        {
                            "dirty",
                            new V6.AmpToneOverride
                            {
                                Gain = g
                            }
                        }
                    }
                }
            };
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

        public class SongName
        {
            public List<string> Names { get; set; }
            public string ShortName { get; set; }
            public string Starts { get; set; }
        }

        public class SongNames
        {
            public List<SongName> Songs { get; set; }
        }
    }
}
