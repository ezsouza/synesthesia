using System;
using System.Collections.Generic;
using System.Linq;

namespace Synesthesia.App.Models
{
    public class Playlist
    {
        private readonly List<Track> tracks = new();
        private readonly Random random = new();
        
        public event EventHandler<TrackChangedEventArgs>? TrackChanged;
        
        public IReadOnlyList<Track> Tracks => tracks.AsReadOnly();
        public Track? CurrentTrack { get; private set; }
        public int CurrentIndex { get; private set; } = -1;
        public bool IsShuffleEnabled { get; set; }
        public bool IsRepeatEnabled { get; set; }
        public bool IsRepeatOneEnabled { get; set; }

        public void AddTrack(Track track)
        {
            tracks.Add(track);
        }

        public void AddTracks(IEnumerable<Track> tracksToAdd)
        {
            tracks.AddRange(tracksToAdd);
        }

        public void RemoveTrack(Track track)
        {
            int index = tracks.IndexOf(track);
            if (index >= 0)
            {
                tracks.RemoveAt(index);
                if (index <= CurrentIndex)
                {
                    CurrentIndex--;
                }
            }
        }

        public void Clear()
        {
            tracks.Clear();
            CurrentTrack = null;
            CurrentIndex = -1;
        }

        public Track? GetNextTrack()
        {
            if (tracks.Count == 0) return null;

            if (IsRepeatOneEnabled && CurrentTrack != null)
            {
                return CurrentTrack;
            }

            if (IsShuffleEnabled)
            {
                CurrentIndex = random.Next(tracks.Count);
            }
            else
            {
                CurrentIndex++;
                if (CurrentIndex >= tracks.Count)
                {
                    if (IsRepeatEnabled)
                    {
                        CurrentIndex = 0;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            CurrentTrack = tracks[CurrentIndex];
            TrackChanged?.Invoke(this, new TrackChangedEventArgs(CurrentTrack));
            return CurrentTrack;
        }

        public Track? GetPreviousTrack()
        {
            if (tracks.Count == 0) return null;

            if (IsRepeatOneEnabled && CurrentTrack != null)
            {
                return CurrentTrack;
            }

            if (IsShuffleEnabled)
            {
                CurrentIndex = random.Next(tracks.Count);
            }
            else
            {
                CurrentIndex--;
                if (CurrentIndex < 0)
                {
                    if (IsRepeatEnabled)
                    {
                        CurrentIndex = tracks.Count - 1;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            CurrentTrack = tracks[CurrentIndex];
            TrackChanged?.Invoke(this, new TrackChangedEventArgs(CurrentTrack));
            return CurrentTrack;
        }

        public void SetCurrentTrack(Track track)
        {
            CurrentIndex = tracks.IndexOf(track);
            if (CurrentIndex >= 0)
            {
                CurrentTrack = track;
                TrackChanged?.Invoke(this, new TrackChangedEventArgs(CurrentTrack));
            }
        }

        public void SetCurrentTrack(int index)
        {
            if (index >= 0 && index < tracks.Count)
            {
                CurrentIndex = index;
                CurrentTrack = tracks[index];
                TrackChanged?.Invoke(this, new TrackChangedEventArgs(CurrentTrack));
            }
        }
    }

    public class TrackChangedEventArgs : EventArgs
    {
        public Track Track { get; }

        public TrackChangedEventArgs(Track track)
        {
            Track = track;
        }
    }
}
