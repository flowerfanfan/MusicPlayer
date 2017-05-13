using MusicPlayer.DataBase;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace MusicPlayer.ViewModels
{
    public class LocalSongsVM
    {
        public DataBaseManager DBManager { get; set; }
        public ObservableCollection<Song> Songs { get; set; }
        public ObservableCollection<Song> SelectedSongs { get; set; }

        public LocalSongsVM()
        {
            DBManager = DataBaseManager.GetDBManager();
            SelectedSongs = new ObservableCollection<Song>();
        }

        // 从数据库加载歌曲信息
        public void LoadSongs()
        {
            Songs = DBManager.GetSongs("_Songs_");
            // 第一次启动时
            if (Songs.Count == 0) ReloadSongs();
        }

        // 重新读取本地歌曲信息
        public async void ReloadSongs()
        {
            // 清空数据
            Songs.Clear();
            DBManager.ClearSongs("_Songs_");
            // 读取“音乐”文件夹根目录及子文件夹内的歌曲信息
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".mp3");
            var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            var query = KnownFolders.MusicLibrary.CreateFileQueryWithOptions(queryOptions);
            IReadOnlyList<StorageFile> fileList = await query.GetFilesAsync();
            ReadMusicFiles(fileList);
        }

        // 读取音乐文件的信息
        private async void ReadMusicFiles(IReadOnlyList<StorageFile> fileList)
        {
            foreach (StorageFile file in fileList)
            {
                MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView);
                Songs.Add(new Song(file.Path, musicProperties, thumbnail));
            }
            DBManager.AddSongs(Songs, "_Songs_");
        }
        
        // 批量删除选中的歌曲
        public void DeleteSelectedSongs()
        {
            foreach (Song song in SelectedSongs)
            {
                Songs.Remove(song);
                // 在数据库中删除
                DBManager.DeleteSong("_Songs_", song);
                // TODO:删除本地文件

            }
        }

        // 判断再删除时用户是否已选中歌单
        public bool HasSelected()
        {
            return SelectedSongs.Count != 0;
        }

        public void SelectSong(Song song)
        {
            SelectedSongs.Add(song);
        }

        public void RemoveSelectedSong(Song song)
        {
            SelectedSongs.Remove(song);
        }
    }
}