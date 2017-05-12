using MediaPlayer;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicPlayer.Models
{
    public class Song: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string FilePath { get; set; }
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                }
            }
        }
        private string _artist;
        public string Artist
        {
            get
            {
                return _artist;
            }
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Artist"));
                }
            }
        }
        public string Album { get; set; }
        public string Length { get; set; }
        public BitmapImage Cover { get; set; }
        public byte[] CoverBytes { get; set; }

        //加入歌词作为成员
        public Lyric lyric { get; set; }

        public Song(string  filePath , MusicProperties musicProperties, StorageItemThumbnail thumbnail)
        {
            FilePath = filePath;
            Title = musicProperties.Title;
            Artist = musicProperties.Artist;
            Album = musicProperties.Album;
            Length = GetLength(musicProperties.Duration);
            Cover = new BitmapImage();
            
            Cover.SetSource(thumbnail);
            // 将StorageItemThumbnail转为byte[]
            ConvertToBytes(thumbnail);
        }

        //设置空构造函数，方便构造Song用于传递
        public Song()
        {
            
        }

        private async void ConvertToBytes(StorageItemThumbnail thumbnail)
        {
            var stream = thumbnail.CloneStream();
            CoverBytes = new byte[stream.Size];
            var dataReader = new DataReader(stream);
            await dataReader.LoadAsync((uint)stream.Size);
            dataReader.ReadBytes(CoverBytes);
        }

        private string GetLength(TimeSpan duration)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (duration.Minutes < 10) stringBuilder.Append("0");
            stringBuilder.Append(duration.Minutes);
            stringBuilder.Append(":");
            if (duration.Seconds < 10) stringBuilder.Append("0");
            stringBuilder.Append(duration.Seconds);
            return stringBuilder.ToString();
        }

        public Song(string filePath, string title, string artist, string album, string length, byte[] coverBytes)
        {
            FilePath = filePath;
            Title = title;
            Artist = artist;
            Album = album;
            Length = length;
            CoverBytes = coverBytes;
            // 将byte[]转为BitmapImage
            ConvertToBitmapImage();
        }

        private async void ConvertToBitmapImage()
        {
            Cover = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(CoverBytes.AsBuffer());
                stream.Seek(0);
                await Cover.SetSourceAsync(stream);
            }
        }
    }
}
