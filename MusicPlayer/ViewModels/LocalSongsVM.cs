using MusicPlayer.DataBase;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Search;

namespace MusicPlayer.ViewModels
{
    public class LocalSongsVM
    {
        public DataBaseManager DBManager { get; set; }
        public ObservableCollection<Song> Songs { get; set; }

        public LocalSongsVM()
        {
            DBManager = DataBaseManager.GetDBManager();
        }

        // 从数据库加载歌曲信息
        public void LoadSongs()
        {
            Songs = DBManager.GetSongs("Songs");
            // 第一次启动时
            if (Songs.Count == 0) ReloadSongs();
        }

        // 重新读取本地歌曲信息
        public async void ReloadSongs()
        {
            // 清空数据
            Songs.Clear();
            DBManager.ClearSongs("Songs");
            // 读取“音乐”文件夹根目录及子文件夹内的歌曲信息
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".mp3");
            fileTypeFilter.Add(".wma");
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            var query = KnownFolders.MusicLibrary.CreateFileQueryWithOptions(queryOptions);
            IReadOnlyList<StorageFile> fileList = await query.GetFilesAsync();
            ReadMusicFiles(fileList);
        }

        private async void ReadMusicFiles(IReadOnlyList<StorageFile> fileList)
        {
            foreach (StorageFile file in fileList)
            {
                MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView);
                Songs.Add(new Song(file.Path, musicProperties, thumbnail));
            }
            DBManager.AddSongs(Songs, "Songs");
        }
    }
}
