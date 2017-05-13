using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //根据路径判断是否相等
        //找不到的话返回-1
        static public int getIndexOf(this ObservableCollection<Song> songs, Song m)
        {
            foreach (Song s in songs)
            {
                if (s.FilePath == m.FilePath) return songs.IndexOf(s);
            }
            return -1;
        }

    }
}
