using MusicPlayer.DataBase;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
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
            Songs = DBManager.GetData();
            // 第一次启动时
            if (Songs.Count == 0) ReloadSongs();
        }

        // 重新读取本地歌曲信息
        public async void ReloadSongs()
        {
            // 清空数据
            Songs.Clear();
            DBManager.ClearData();
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
                Songs.Add(new Song(file.Path, musicProperties));
            }
            DBManager.AddSongs(Songs);
        }
    }
}
