using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EMinor.V6;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static System.IO.File;
using static System.Math;

namespace EMinor
{
    public class Controller
    {
        private readonly MidiState midi;
        private readonly int channel;

        private int currentScene;
        private Song currentSong;
        private int currentSongIdx;
        private Setlist currentSetlist;
        private int currentSetlistIdx;

        public Controller(IMIDI midi, int channel)
        {
            // Wrap the MIDI output device in a state-tracker:
            this.midi = (midi is MidiState) ? (MidiState)midi : new MidiState(midi);
            this.channel = channel;

            this.currentSongIdx = 0;
            this.currentSetlistIdx = 0;
            this.currentSetlist = null;
            this.currentSong = null;
            this.currentScene = 0;
        }

        public List<MidiProgram> MidiPrograms { get; private set; }
        public List<Setlist> Setlists { get; private set; }
        public List<Song> Songs { get; private set; }

        public int CurrentSongIndex { get { return currentSongIdx; } }
        public Song CurrentSong { get { return currentSong; } }
        public int CurrentScene { get { return currentScene; } }
        public int LastScene { get { return currentSong.SceneDescriptors.Count - 1; } }

        public Setlist CurrentSetlist { get { return currentSetlist; } set { currentSetlist = value; } }
        public int CurrentSetlistIndex { get { return currentSetlistIdx; } }
        public int LastSetlistIndex { get { return (currentSetlist?.Songs?.Count ?? 1) - 1; } }

        public string CurrentSongName => (CurrentSetlist != null
            ? (currentSetlistIdx + 1).ToString()
            : (currentSongIdx + 1).ToString())
            + ". " + CurrentSong?.Name ?? "";

        public int LastSongIndex => Songs.Count - 1;

        public string CurrentSceneDisplay => String.Format("{0,2}/{1,2}", CurrentScene + 1, LastScene + 1);

        public void ActivateSetlist(Setlist setlist)
        {
            currentSetlist = setlist;
            currentSetlistIdx = 0;
            if (currentSetlistIdx < setlist.Songs.Count)
            {
                ActivateSong(setlist.Songs[currentSetlistIdx], 0);
            }
        }

        public void MidiReset()
        {
            midi.Reset();
            midi.StartBatch();
            ActivateSong(CurrentSong, CurrentScene);
            midi.EndBatch();
        }

        public void NextScene()
        {
            if (CurrentScene < LastScene)
            {
                // advance to next scene in current song:
                ActivateScene(CurrentScene + 1);
                return;
            }

            var nextSong = (Song)null;

            if (CurrentSetlist != null)
            {
                // setlist mode:
                currentSetlistIdx++;
                if (currentSetlistIdx > LastSetlistIndex)
                {
                    // Don't wrap; end of setlist:
                    currentSetlistIdx = LastSetlistIndex;
                    return;
                }

                nextSong = CurrentSetlist.Songs[currentSetlistIdx];
            }
            else
            {
                // song mode:
                currentSongIdx++;
                if (currentSongIdx > LastSongIndex)
                {
                    // Wrap around to first song:
                    currentSongIdx = 0;
                }

                nextSong = Songs[currentSongIdx];
            }

            ActivateSong(nextSong, 0);
        }

        public void PreviousScene()
        {
            if (CurrentScene > 0)
            {
                // advance to previous scene in current song:
                ActivateScene(CurrentScene - 1);
                return;
            }

            var nextSong = (Song)null;

            if (CurrentSetlist != null)
            {
                // setlist mode:
                currentSetlistIdx--;
                if (currentSetlistIdx < 0)
                {
                    currentSetlistIdx = 0;
                    return;
                }

                nextSong = CurrentSetlist.Songs[currentSetlistIdx];
            }
            else
            {
                // song mode:
                currentSongIdx--;
                if (currentSongIdx < 0)
                {
                    // wrap around to last song:
                    currentSongIdx = LastSongIndex;
                }

                nextSong = Songs[currentSongIdx];
            }

            // Select last scene in previous song:
            int nextScene = nextSong.SceneDescriptors.Count - 1;
            ActivateSong(nextSong, nextScene);
        }

        public void NextSong()
        {
            var nextSong = (Song)null;

            if (CurrentSetlist != null)
            {
                // setlist mode:
                currentSetlistIdx++;
                if (currentSetlistIdx > LastSetlistIndex)
                {
                    // Don't wrap; end of setlist:
                    currentSetlistIdx = LastSetlistIndex;
                    return;
                }

                nextSong = CurrentSetlist.Songs[currentSetlistIdx];
            }
            else
            {
                // song mode:
                currentSongIdx++;
                if (currentSongIdx > LastSongIndex)
                {
                    // Wrap around to first song:
                    currentSongIdx = 0;
                }

                nextSong = Songs[currentSongIdx];
            }

            ActivateSong(nextSong, 0);
        }

        public void PreviousSong()
        {
            var nextSong = (Song)null;

            if (CurrentSetlist != null)
            {
                // setlist mode:
                currentSetlistIdx--;
                if (currentSetlistIdx < 0)
                {
                    currentSetlistIdx = 0;
                    return;
                }

                nextSong = CurrentSetlist.Songs[currentSetlistIdx];
            }
            else
            {
                // song mode:
                currentSongIdx--;
                if (currentSongIdx < 0)
                {
                    // wrap around to last song:
                    currentSongIdx = LastSongIndex;
                }

                nextSong = Songs[currentSongIdx];
            }

            // Select first scene in previous song:
            ActivateSong(nextSong, 0);
        }

        public void StartMidiBatch()
        {
            midi.StartBatch();
        }

        public void EndMidiBatch()
        {
            midi.EndBatch();
        }

        public void LoadData()
        {
            var de = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            AllPrograms allPrograms;
            using (var tr = OpenText("all-programs-v6.yml"))
                allPrograms = de.Deserialize<V6.AllPrograms>(tr);

            this.MidiPrograms = allPrograms.MidiPrograms;

            Setlists setlists;
            using (var tr = OpenText("setlists.yml"))
                setlists = de.Deserialize<Setlists>(tr);

            this.Setlists = setlists.Sets;

            // Set back-references since we can't really deserialize these:
            foreach (var midiProgram in MidiPrograms)
            {
                if (midiProgram.Amps.Count == 0)
                {
                    throw new Exception("MIDI program must define at least one amp!");
                }

                for (int i = 0; i < midiProgram.Amps.Count; i++)
                {
                    var ampDefinition = midiProgram.Amps[i];
                    ampDefinition.AmpNumber = i;
                    ampDefinition.MidiProgram = midiProgram;

                    foreach (var tone in ampDefinition.Tones.Values)
                    {
                        tone.AmpDefinition = ampDefinition;
                        tone.Volume = DBtoMIDI(tone.VolumeDB);

                        foreach (var blockName in ampDefinition.Blocks.Keys)
                        {
                            if (!tone.Blocks.ContainsKey(blockName))
                            {
                                // Enforce default on/off X/Y state for all blocks defined for the amp:
                                tone.Blocks.Add(blockName, new FXBlock
                                {
                                    On = false,
                                    XY = XYSwitch.X
                                });
                            }
                        }
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
                            SongAmpOverrides amp = song.Amps[i];
                            amp.AmpNumber = i;
                            amp.AmpDefinition = midiProgram.Amps[i];

                            foreach (var toneKey in amp.Tones.Keys)
                            {
                                var tone = amp.Tones[toneKey];
                                if (!midiProgram.Amps[i].Tones.ContainsKey(toneKey))
                                {
                                    throw new Exception(String.Format("Song '{0}' amp {1} tone '{2}' must exist in MIDI program amp definition!", song.Name, i + 1, toneKey));
                                }

                                // Convert dB to MIDI:
                                tone.Volume = tone.VolumeDB.HasValue ? DBtoMIDI(tone.VolumeDB.Value) : (int?)null;
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
                            SceneAmpToneSelection amp = scene.Amps[i];
                            if (!midiProgram.Amps[i].Tones.ContainsKey(amp.Tone))
                            {
                                throw new Exception(string.Format("Song '{0}' scene '{1}' amp {2} tone '{3}' must exist in MIDI program amp definition!", song.Name, scene.Name, i + 1, amp.Tone));
                            }

                            amp.AmpNumber = i;
                            amp.AmpToneDefinition = midiProgram.Amps[i].Tones[amp.Tone];

                            // Convert dB to MIDI:
                            amp.Volume = amp.VolumeDB.HasValue ? DBtoMIDI(amp.VolumeDB.Value) : (int?)null;
                        }
                    }
                }
            }

            // Sort songs alphabetically across all MIDI programs:
            Songs = MidiPrograms
                .SelectMany(m => m.Songs)
                .OrderBy(s => s.Name)
                .ToList();

            // Initialize Setlists:
            foreach (var setlist in Setlists)
            {
                // Match song names with songs:
                setlist.Songs = new List<V6.Song>(setlist.SongNames.Count);
                for (int i = 0; i < setlist.SongNames.Count; i++)
                {
                    var setlistSongName = setlist.SongNames[i];

                    // Skip BREAK markers used for printouts:
                    if (setlistSongName.StartsWith("BREAK:", StringComparison.OrdinalIgnoreCase)) continue;

                    var song = (
                        from s in Songs
                        where s.MatchesName(setlistSongName)
                        select s
                    ).SingleOrDefault();

                    if (song == null)
                    {
                        Console.Error.WriteLine("Setlist {0} song name '{1}' has no match in songs!", setlist.Date, setlistSongName);
                        continue;
                    }

                    setlist.Songs.Add(song);
                }
            }
        }

        static double MIDItoDB(int volume)
        {
            return 0;
        }

        static int DBtoMIDI(double db)
        {
            db = db - 6.0;
            double p = Pow(10.0, (db / 20.0));
            double plog = Log10(p * 14.5 + 1.0) / Log10(15.5);
            plog *= 127.0;
            return (int)(Round(plog));
        }

        public void ActivateSong(Song newSong, int scene)
        {
            Trace.WriteLine($"Activate song '{newSong.Name}'");

            Trace.WriteLine($"Change MIDI program {newSong.MidiProgram.ProgramNumber}");
            // HACK: add 10 for new scenes
            midi.SetProgram(channel, newSong.MidiProgram.ProgramNumber + 10);

            this.currentSong = newSong;
            ActivateScene(scene);
        }

        public void ActivateScene(int scene)
        {
            this.currentScene = scene;
            Trace.WriteLine($"Activate song '{currentSong.Name}' scene {currentScene}");

            if (currentScene >= currentSong.SceneDescriptors.Count)
            {
                throw new Exception("Invalid scene number!");
            }

            // TODO: move this state to class level so it can be reported to UI.
            for (int i = 0; i < currentSong.MidiProgram.Amps.Count; i++)
            {
                SceneDescriptor sceneDescriptor = currentSong.SceneDescriptors[currentScene];
                SceneAmpToneSelection toneSelection = sceneDescriptor.Amps[i];
                AmpToneDefinition toneDefinition = toneSelection.AmpToneDefinition;
                AmpDefinition ampDefinition = toneDefinition.AmpDefinition;

                // Figure out the song-specific override of tone:
                SongAmpToneOverride toneOverride = currentSong.Amps?[i].Tones?.GetValueOrDefault(toneSelection.Tone);

                // Combine tone definition with scene tone override:
                var blockNames = toneDefinition.Blocks.Keys.ToHashSet();
                blockNames.UnionWith(toneSelection.Blocks?.Keys ?? Enumerable.Empty<string>());

                // Set the gain and volume:
                var gain = toneSelection.Gain ?? toneOverride?.Gain ?? toneDefinition.Gain;
                var volumeMIDI = toneSelection.Volume ?? toneOverride?.Volume ?? toneDefinition.Volume;

                Trace.WriteLine($"Amp[{i + 1}]: gain   val (CC {toneDefinition.AmpDefinition.GainControllerCC:X2}h) to {gain:X2}h");
                midi.SetController(channel, toneDefinition.AmpDefinition.GainControllerCC, gain);
                Trace.WriteLine($"Amp[{i + 1}]: volume val (CC {toneDefinition.AmpDefinition.VolumeControllerCC:X2}h) to {volumeMIDI:X2}h ({MIDItoDB(volumeMIDI)} dB)");
                midi.SetController(channel, toneDefinition.AmpDefinition.VolumeControllerCC, volumeMIDI);

                // Set all the controller values for the selected tone:
                foreach (string blockName in blockNames)
                {
                    FXBlock blockDefault;
                    toneDefinition.Blocks.TryGetValue(blockName, out blockDefault);
                    FXBlockDefinition blockDefinition = ampDefinition.Blocks[blockName];

                    int enabledCC = blockDefinition.EnabledSwitchCC;
                    int? xySwitchCC = blockDefinition.XYSwitchCC;

                    SongFXBlockOverride songBlockOverride = toneOverride?.Blocks?.GetValueOrDefault(blockName);

                    // Follow inheritance chain to determine enabled and X/Y switch values:
                    var sceneBlockOverride = toneSelection.Blocks?.GetValueOrDefault(blockName);

                    var enabled = sceneBlockOverride?.On ?? songBlockOverride?.On ?? blockDefault?.On;
                    var xy = sceneBlockOverride?.XY ?? songBlockOverride?.XY ?? blockDefault?.XY;

                    if (enabled.HasValue)
                    {
                        Trace.WriteLine($"Amp[{i + 1}]: {blockName}   1/0 (CC {enabledCC:X2}h) to {(enabled.Value ? "on" : "off")}");
                        midi.SetController(channel, enabledCC, enabled.Value ? 0x7F : 0x00);
                    }
                    if (xy.HasValue && xySwitchCC.HasValue)
                    {
                        Trace.WriteLine($"Amp[{i + 1}]: {blockName}   X/Y (CC {xySwitchCC.Value:X2}h) to {(enabled.Value ? "X" : "Y")}");
                        midi.SetController(channel, xySwitchCC.Value, xy.Value == XYSwitch.X ? 0x7F : 0x00);
                    }
                }

            }

            // TODO: send tempo SysEx message.
        }
    }
}
