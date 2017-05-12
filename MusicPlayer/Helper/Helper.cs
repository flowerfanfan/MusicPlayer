using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaPlayer
{
    static class Helper
    {
        static public bool isAudio(StorageFile file)
        {
            return (file.FileType != ".mp4" && file.FileType != ".mkv" && file.FileType != ".avi");
        }
        static public async Task<Song> ToSong(this StorageFile file)
        {
            var properties = await file.Properties.GetMusicPropertiesAsync();
            var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.MusicView);
            BitmapImage tn = new BitmapImage();
            tn.SetSource(thumbnail);
            Song s = new Song(file.Path, properties, thumbnail);
            s.lyric = new Lyric();
            s.Cover = tn;
            return s;
        }
    }
}
