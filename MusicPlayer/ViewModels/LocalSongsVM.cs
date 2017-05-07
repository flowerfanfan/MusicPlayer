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
            // 读取“音乐”文件夹根目录下的音乐文件
            StorageFolder musicFolder = KnownFolders.MusicLibrary;
            IReadOnlyList<StorageFile> fileList = await musicFolder.GetFilesAsync();
            ReadMusicFiles(fileList);
            // 读取“音乐”文件夹的各子文件夹根目录下的音乐文件
            IReadOnlyList<StorageFolder> folderList = await musicFolder.GetFoldersAsync();
            foreach (StorageFolder folder in folderList)
            {
                IReadOnlyList<StorageFile> folderFileList = await folder.GetFilesAsync();
                ReadMusicFiles(folderFileList);
            }
        }

        private async void ReadMusicFiles(IReadOnlyList<StorageFile> fileList)
        {
            foreach (StorageFile file in fileList)
            {
                switch (file.FileType)
                {
                    // 过滤文件类型
                    case ".mp3":
                        MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                        Songs.Add(new Song(file.Path, musicProperties));
                        break;
                    default:
                        break;
                }
            }
            DBManager.AddSongs(Songs);
        }
    }
}
