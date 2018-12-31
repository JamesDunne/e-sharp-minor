using System;
using System.Collections.Generic;
using static System.IO.File;
using static System.Math;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;

namespace EMinor
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

            Console.WriteLine("Reading 'all-programs-v5.yml'");
            using (var tr = OpenText("all-programs-v5.yml"))
                v5programs = de.Deserialize<V5.AllPrograms>(tr);

            Console.WriteLine("Reading 'song-names.yml'");
            using (var tr = OpenText("song-names.yml"))
                songNames = de.Deserialize<V5.SongNames>(tr);

            Console.WriteLine("Translating V5 to V6...");
            var v6programs = convertV5toV6();

            var se = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            Console.WriteLine("Writing 'all-programs-v6.yml'");
            using (var tw = CreateText("all-programs-v6.yml"))
                se.Serialize(tw, v6programs);

            Console.WriteLine("Done");
        }

        static int logTaper(int b)
        {
            // 127 * (ln(x+1)^2) / (ln(127+1)^2)
            return (int)(127.0 * Pow(Math.Log((double)(b) + 1.0), 2) / Pow(Log(127.0 + 1.0), 2));
        }

        private V6.SceneAmpToneSelection convertAmp(V5.Amp amp)
        {
            return new V6.SceneAmpToneSelection
            {
                Tone = amp.Channel,
                Gain = amp.Gain == 0 ? (amp.GainLog == 0 ? (int?)null : logTaper(amp.GainLog)) : amp.Gain,
                VolumeDB = amp.Level == 0 ? (double?)null : amp.Level,
                Blocks = amp.FX?.Select(
                    fx => new V6.SongFXBlockOverride
                    {
                        Name = fx,
                        On = true
                    }
                ).ToList()
            };
        }

        static int? maybeGain(int gain, int gainLog)
        {
            return gain == 0 ? (gainLog == 0 ? (int?)null : logTaper(gainLog)) : gain;
        }

        public class ToneV5 : IEquatable<ToneV5>
        {
            public bool IsDirty;
            public int? Gain;
            public double? VolumeDB;
            public HashSet<string> FX;

            public override bool Equals(object obj)
            {
                return Equals(obj as ToneV5);
            }

            public bool Equals(ToneV5 other)
            {
                return other != null &&
                       IsDirty == other.IsDirty &&
                       EqualityComparer<int?>.Default.Equals(Gain, other.Gain) &&
                       EqualityComparer<double?>.Default.Equals(VolumeDB, other.VolumeDB) &&
                       EqualityComparer<HashSet<string>>.Default.Equals(FX, other.FX);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(IsDirty, Gain, VolumeDB, FX);
            }

            public static bool operator ==(ToneV5 v1, ToneV5 v2)
            {
                return EqualityComparer<ToneV5>.Default.Equals(v1, v2);
            }

            public static bool operator !=(ToneV5 v1, ToneV5 v2)
            {
                return !(v1 == v2);
            }
        }

        private List<V6.SongAmpOverrides> createSongAmps(V5.Program p)
        {
            // Song-default dirty gain level:
            int? g = maybeGain(p.Gain, p.GainLog);

            // Create distinct tones for each amp:
            var mgTones = (
                from s in p.SceneDescriptors
                select tone(g, s.MG)
            ).Distinct().ToList();

            var jdTones = (
                from s in p.SceneDescriptors
                select tone(g, s.JD)
            ).Distinct().ToList();

            // These instances must be separate to avoid YAML serializer aliasing:
            return new List<V6.SongAmpOverrides>
            {
                // MG:
                new V6.SongAmpOverrides
                {
                    Tones = mgTones.Select((t, i) => new V6.SongAmpToneOverride {
                        Name = (t.IsDirty ? "dirty" : "clean") + i.ToString(),
                        VolumeDB = t.VolumeDB,
                        Gain = t.Gain,
                        Blocks = t.FX?.Select(fx => new V6.SongFXBlockOverride{ Name = fx, On = true }).ToList()
                    }).ToList()
                },
                // JD:
                new V6.SongAmpOverrides
                {
                    Tones = jdTones.Select((t, i) => new V6.SongAmpToneOverride {
                        Name = (t.IsDirty ? "dirty" : "clean") + i.ToString(),
                        VolumeDB = t.VolumeDB,
                        Gain = t.Gain,
                        Blocks = t.FX?.Select(fx => new V6.SongFXBlockOverride{ Name = fx, On = true }).ToList()
                    }).ToList()
                }
            };
        }

        private static ToneV5 tone(int? g, V5.Amp amp)
        {
            return new ToneV5
            {
                IsDirty = amp.Channel == "dirty",
                Gain = maybeGain(amp.Gain, amp.GainLog) ?? (amp.Channel == "dirty" ? g : null),
                VolumeDB = amp.Level == 0.0 ? (double?)null : amp.Level,
                FX = amp.FX?.ToHashSet()
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
                                Blocks = new List<V6.FXBlockDefinition>
                                {
                                    new V6.FXBlockDefinition { Name = "amp1", EnabledSwitchCC = 37, XYSwitchCC = 100 },
                                    new V6.FXBlockDefinition { Name = "cab1", EnabledSwitchCC = 39, XYSwitchCC = 102 },
                                    new V6.FXBlockDefinition { Name = "gte1", EnabledSwitchCC = 60 },
                                    new V6.FXBlockDefinition { Name = "cmp1", EnabledSwitchCC = 43 },
                                    new V6.FXBlockDefinition { Name = "pit1", EnabledSwitchCC = 77 },
                                    new V6.FXBlockDefinition { Name = "rtr1", EnabledSwitchCC = 86 },
                                    new V6.FXBlockDefinition { Name = "phr1", EnabledSwitchCC = 75 },
                                    new V6.FXBlockDefinition { Name = "cho1", EnabledSwitchCC = 41 },
                                    new V6.FXBlockDefinition { Name = "dly1", EnabledSwitchCC = 47 }
                                },
                                Tones = new List<V6.AmpToneDefinition>
                                {
                                    new V6.AmpToneDefinition {
                                        Name = "clean",
                                        Gain = 0x12,
                                        VolumeDB = 0,
                                        Blocks = new List<V6.FXBlock>
                                        {
                                            new V6.FXBlock { Name = "amp1", On = true, XY = V6.XYSwitch.Y },
                                            new V6.FXBlock { Name = "cab1", On = true, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "gte1", On = false, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "cmp1", On = true, XY = V6.XYSwitch.X }
                                        }
                                    },
                                    new V6.AmpToneDefinition {
                                        Name = "dirty",
                                        Gain = 0x40,
                                        VolumeDB = 0,
                                        Blocks = new List<V6.FXBlock>
                                        {
                                            new V6.FXBlock { Name = "amp1", On = true, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "cab1", On = true, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "gte1", On = true, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "cmp1", On = true, XY = V6.XYSwitch.X }
                                        }
                                    },
                                    new V6.AmpToneDefinition {
                                        Name = "acoustic",
                                        Gain = 0x12,
                                        VolumeDB = 0,
                                        Blocks = new List<V6.FXBlock>
                                        {
                                            new V6.FXBlock { Name = "amp1", On = false, XY = V6.XYSwitch.Y },
                                            new V6.FXBlock { Name = "cab1", On = true, XY = V6.XYSwitch.Y },
                                            new V6.FXBlock { Name = "gte1", On = false, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "cmp1", On = true, XY = V6.XYSwitch.X }
                                        }
                                    }
                                },
                                VolumeControllerCC = 16,
                                GainControllerCC = 18
                            },
                            // JD:
                            new V6.AmpDefinition
                            {
                                Name = "JD",
                                Blocks = new List<V6.FXBlockDefinition>
                                {
                                    new V6.FXBlockDefinition { Name = "amp2", EnabledSwitchCC = 38, XYSwitchCC = 101 },
                                    new V6.FXBlockDefinition { Name = "cab2", EnabledSwitchCC = 40, XYSwitchCC = 103 },
                                    new V6.FXBlockDefinition { Name = "gte2", EnabledSwitchCC = 61 },
                                    new V6.FXBlockDefinition { Name = "cmp2", EnabledSwitchCC = 44 },
                                    new V6.FXBlockDefinition { Name = "pit2", EnabledSwitchCC = 78 },
                                    new V6.FXBlockDefinition { Name = "rtr2", EnabledSwitchCC = 87 },
                                    new V6.FXBlockDefinition { Name = "phr2", EnabledSwitchCC = 76 },
                                    new V6.FXBlockDefinition { Name = "cho2", EnabledSwitchCC = 42 },
                                    new V6.FXBlockDefinition { Name = "dly2", EnabledSwitchCC = 48 }
                                },
                                Tones = new List<V6.AmpToneDefinition>
                                {
                                    new V6.AmpToneDefinition {
                                        Name = "clean",
                                        Gain = 0x12,
                                        VolumeDB = 0,
                                        Blocks = new List<V6.FXBlock>
                                        {
                                            new V6.FXBlock { Name = "amp2", On = true, XY = V6.XYSwitch.Y },
                                            new V6.FXBlock { Name = "cab2", On = true, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "gte2", On = false, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "cmp2", On = true, XY = V6.XYSwitch.X }
                                        }
                                    },
                                    new V6.AmpToneDefinition {
                                        Name = "dirty",
                                        Gain = 0x40,
                                        VolumeDB = 0,
                                        Blocks = new List<V6.FXBlock>
                                        {
                                            new V6.FXBlock { Name = "amp2", On = true, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "cab2", On = true, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "gte2", On = true, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "cmp2", On = true, XY = V6.XYSwitch.X }
                                        }
                                    },
                                    new V6.AmpToneDefinition {
                                        Name = "acoustic",
                                        Gain = 0x12,
                                        VolumeDB = 0,
                                        Blocks = new List<V6.FXBlock>
                                        {
                                            new V6.FXBlock { Name = "amp2", On = false, XY = V6.XYSwitch.Y },
                                            new V6.FXBlock { Name = "cab2", On = true, XY = V6.XYSwitch.Y },
                                            new V6.FXBlock { Name = "gte2", On = false, XY = V6.XYSwitch.X },
                                            new V6.FXBlock { Name = "cmp2", On = true, XY = V6.XYSwitch.X }
                                        }
                                    }
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
                            let amps = createSongAmps(p)  // defaultAmps(p)
                            select new V6.Song
                            {
                                Name = p.Name,
                                ShortName = alternateNames.ShortName,
                                AlternateNames = alternateNames.Names,
                                WhoStarts = alternateNames.Starts,
                                Tempo = p.Tempo,
                                Amps = amps,
                                SceneDescriptors = (
                                    from s in p.SceneDescriptors
                                    select new V6.SceneDescriptor
                                    {
                                        Name = s.Name,
                                        Amps = new List<V6.SceneAmpToneSelection>
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

        private List<V6.SongAmpOverrides> defaultAmps(V5.Program p)
        {
            int? g = p.Gain == 0 ? (p.GainLog == 0 ? (int?)null : logTaper(p.GainLog)) : p.Gain;
            if (!g.HasValue) return null;

            // These instances must be separate to avoid YAML serializer aliasing:
            return new List<V6.SongAmpOverrides>
            {
                new V6.SongAmpOverrides
                {
                    Tones = new List<V6.SongAmpToneOverride>
                    {
                        new V6.SongAmpToneOverride
                        {
                            Name = "dirty",
                            Gain = g
                        }
                    }
                },
                new V6.SongAmpOverrides
                {
                    Tones = new List<V6.SongAmpToneOverride>
                    {
                        new V6.SongAmpToneOverride
                        {
                            Name = "dirty",
                            Gain = g
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
