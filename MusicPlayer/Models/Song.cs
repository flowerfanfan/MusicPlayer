using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;

namespace MusicPlayer
{
    class Song
    {
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Length { get; set; }

        public Song(string filePath, MusicProperties musicProperties)
        {
            FilePath = filePath;
            Title = musicProperties.Title;
            Artist = musicProperties.Artist;
            Album = musicProperties.Album;
            Length = getLength(musicProperties.Duration);
        }

        private string getLength(TimeSpan duration)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (duration.Minutes < 10) stringBuilder.Append("0");
            stringBuilder.Append(duration.Minutes);
            stringBuilder.Append(":");
            if (duration.Seconds < 10) stringBuilder.Append("0");
            stringBuilder.Append(duration.Seconds);
            return stringBuilder.ToString();
        }

        public Song(string filePath, string title, string artist, string album, string length)
        {
            FilePath = filePath;
            Title = title;
            Artist = artist;
            Album = album;
            Length = length;
        }
    }
}
