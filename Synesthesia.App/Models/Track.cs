using System;

namespace Synesthesia.App.Models
{
    public class Track
    {
        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public TimeSpan Duration { get; set; }
        public string FilePath { get; set; } = "";
        public string Genre { get; set; } = "";
        public int Year { get; set; }

        public Track() { }

        public Track(string filePath, string title = "", string artist = "")
        {
            FilePath = filePath;
            Title = !string.IsNullOrEmpty(title) ? title : System.IO.Path.GetFileNameWithoutExtension(filePath);
            Artist = !string.IsNullOrEmpty(artist) ? artist : "Unknown Artist";
        }

        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }
    }
}
