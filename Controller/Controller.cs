﻿using System;
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
        private List<LiveAmp> liveAmps;

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

        public List<LiveAmp> LiveAmps { get { return liveAmps; } }

        public void ActivateSetlist(Setlist setlist)
        {
            currentSetlist = setlist;
            currentSetlistIdx = 0;
            if (currentSetlistIdx < setlist.Songs.Count)
            {
                ActivateSong(setlist.Songs[currentSetlistIdx], 0);
            }
        }

        public void MidiResend()
        {
            // Re-send current MIDI amp state:
            midi.Reset();
            midi.StartBatch();
            foreach (var liveAmp in liveAmps) {
                ActivateLiveAmp(liveAmp);
            }
            //ActivateSong(CurrentSong, CurrentScene);
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

        //  p = 10 ^ (dB / 20)
        // dB = log10(p) * 20
        // Log20A means 20% percent at half-way point of knob, i.e. dB = 20 * ln(0.20) / ln(10) = -13.98dB
        public static double dB(double percent)
        {
            return Log10(percent) * 20.0;
        }

        public static double MIDItoDB(int volume)
        {
            var p = (double)volume / 127.0;
            // log20a taper (50% -> 20%)
            p = (Pow(15.5, p) - 1.0) / 14.5;
            var db = dB(p) + 6.0;
            return db;
        }

        public static int DBtoMIDI(double db)
        {
            db = db - 6.0;
            double p = Pow(10.0, (db / 20.0));
            double plog = Log10(p * 14.5 + 1.0) / Log10(15.5);
            plog *= 127.0;
            return (int)(Round(plog));
        }

        public void ActivateSong(Song newSong, int scene)
        {
            Debug.WriteLine($"Activate song '{newSong.Name}'");

            Debug.WriteLine($"Change MIDI program {newSong.MidiProgram.ProgramNumber}");
            // HACK: add 10 for new scenes
            midi.SetProgram(channel, newSong.MidiProgram.ProgramNumber + 10);

            this.currentSong = newSong;
            ActivateScene(scene);
        }

        public void ActivateLiveAmp(LiveAmp liveAmp)
        {
            Debug.WriteLine($"Amp[{liveAmp.AmpDefinition.AmpNumber + 1}]: gain   val (CC {liveAmp.AmpDefinition.GainControllerCC:X2}h) to {liveAmp.Gain:X2}h");
            midi.SetController(channel, liveAmp.AmpDefinition.GainControllerCC, liveAmp.Gain);
            Debug.WriteLine($"Amp[{liveAmp.AmpDefinition.AmpNumber + 1}]: volume val (CC {liveAmp.AmpDefinition.VolumeControllerCC:X2}h) to {liveAmp.VolumeMIDI:X2}h ({MIDItoDB(liveAmp.VolumeMIDI)} dB)");
            midi.SetController(channel, liveAmp.AmpDefinition.VolumeControllerCC, liveAmp.VolumeMIDI);

            foreach (var liveFX in liveAmp.FX)
            {
                if (liveFX.Enabled.HasValue)
                {
                    Debug.WriteLine($"Amp[{liveAmp.AmpDefinition.AmpNumber + 1}]: {liveFX.BlockName}   1/0 (CC {liveFX.EnabledCC:X2}h) to {(liveFX.Enabled.Value ? "on" : "off")}");
                    midi.SetController(channel, liveFX.EnabledCC, liveFX.Enabled.Value ? 0x7F : 0x00);
                }

                if (liveFX.XY.HasValue && liveFX.XYSwitchCC.HasValue)
                {
                    Debug.WriteLine($"Amp[{liveAmp.AmpDefinition.AmpNumber + 1}]: {liveFX.BlockName}   X/Y (CC {liveFX.XYSwitchCC.Value:X2}h) to {(liveFX.XY.Value == XYSwitch.X ? "X" : "Y")}");
                    midi.SetController(channel, liveFX.XYSwitchCC.Value, liveFX.XY.Value == XYSwitch.X ? 0x7F : 0x00);
                }
            }
        }

        public void ActivateScene(int scene)
        {
            this.currentScene = scene;
            Debug.WriteLine($"Activate song '{currentSong.Name}' scene {currentScene}");

            if (currentScene >= currentSong.SceneDescriptors.Count)
            {
                throw new Exception("Invalid scene number!");
            }

            liveAmps = new List<LiveAmp>(currentSong.MidiProgram.Amps.Count);
            for (int i = 0; i < currentSong.MidiProgram.Amps.Count; i++)
            {
                var liveAmp = new LiveAmp();
                liveAmps.Add(liveAmp);

                liveAmp.SceneDescriptor = currentSong.SceneDescriptors[currentScene];
                liveAmp.SceneTone = liveAmp.SceneDescriptor.Amps[i];
                liveAmp.ToneDefinition = liveAmp.SceneTone.AmpToneDefinition;
                liveAmp.AmpDefinition = liveAmp.ToneDefinition.AmpDefinition;

                // Figure out the song-specific override of tone:
                liveAmp.SongTone = currentSong.Amps?[i].Tones?.GetValueOrDefault(liveAmp.SceneTone.Tone);

                // Set the gain and volume:
                liveAmp.Gain = liveAmp.SceneTone.Gain ?? liveAmp.SongTone?.Gain ?? liveAmp.ToneDefinition.Gain;
                liveAmp.VolumeMIDI = liveAmp.SceneTone.Volume ?? liveAmp.SongTone?.Volume ?? liveAmp.ToneDefinition.Volume;

                // Combine tone definition with scene tone override:
                var blockNames = liveAmp.ToneDefinition.Blocks.Keys.ToHashSet();

                // Set all the controller values for the selected tone:
                liveAmp.FX = (
                    from blockName in blockNames
                    let blockDefinition = liveAmp.AmpDefinition.Blocks[blockName]
                    let blockDefault = liveAmp.ToneDefinition.Blocks[blockName]
                    let songBlockOverride = liveAmp.SongTone?.Blocks?.GetValueOrDefault(blockName)
                    let sceneBlockOverride = liveAmp.SceneTone.Blocks?.GetValueOrDefault(blockName)
                    select new LiveFX
                    {
                        BlockName = blockName,
                        EnabledCC = blockDefinition.EnabledSwitchCC,
                        XYSwitchCC = blockDefinition.XYSwitchCC,
                        Enabled = sceneBlockOverride?.On ?? songBlockOverride?.On ?? blockDefault.On,
                        XY = sceneBlockOverride?.XY ?? songBlockOverride?.XY ?? blockDefault.XY
                    }
                ).ToList();

                // Send MIDI updates:
                ActivateLiveAmp(liveAmp);
            }

            // TODO: send tempo SysEx message.
        }
    }
}
