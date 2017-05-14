using MusicPlayer.DataBase;
using MusicPlayer.Frames;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace MusicPlayer.ViewModels
{
    public class LocalSongsVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        // 因需要与其他页面（MainPage）进行数据交换，故采用单例模式
        private static LocalSongsVM localSongsVM;
        public DataBaseManager DBManager { get; set; }
        public ObservableCollection<Song> _Songs;
        public ObservableCollection<Song> Songs
        {
            get
            {
                return _Songs;
            }
            set
            {
                if (_Songs != value)
                {
                    _Songs = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Songs"));
                }
            }
        }
        public ObservableCollection<Song> SelectedSongs { get; set; }

        private LocalSongsVM()
        {
            DBManager = DataBaseManager.GetDBManager();
            SelectedSongs = new ObservableCollection<Song>();
            LoadSongs();
        }

        // 根据搜索内容返回相关歌曲信息
        public ObservableCollection<Song> SearchSongs(string searchContent)
        {
            ObservableCollection<Song> results = new ObservableCollection<Song>();
            foreach (Song song in Songs)
            {
                if (song.Title.Contains(searchContent) ||
                    song.Artist.Contains(searchContent) ||
                    song.Album.Contains(searchContent))
                {
                    results.Add(song);
                }
            }
            return results;
        }

        // 从数据库加载歌曲信息
        public void LoadSongs()
        {
            Songs = DBManager.GetSongs("_Songs_");
            // 第一次启动时
            if (Songs.Count == 0) ReloadSongs();
        }

        public static LocalSongsVM GetLocalSongsVM()
        {
            if (localSongsVM == null)
            {
                localSongsVM = new LocalSongsVM();
            }
            return localSongsVM;
        }

        // 重新读取本地歌曲信息
        public async void ReloadSongs()
        {
            // 清空数据
            ObservableCollection<Song> tempSongs = new ObservableCollection<Song>(Songs);

            LocalSongs.Current.Frame.IsEnabled = false;

            Songs.Clear();
            DBManager.ClearSongs("_Songs_");
            

            try {
                foreach (Song s in tempSongs) {
                    try
                    {
                        var t = await StorageFile.GetFileFromPathAsync(s.FilePath);
                    }
                    catch (Exception ex)
                    {
                        tempSongs.Remove(s);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            Songs = tempSongs;
            DBManager.AddSongs(Songs, "_Songs_");
            Songs.CollectionChanged += Songs_CollectionChanged;


            LocalSongs.Current.Frame.IsEnabled = true;
            // 读取“音乐”文件夹根目录及子文件夹内的歌曲信息
            /*
             List<string> fileTypeFilter = new List<string>();
             fileTypeFilter.Add(".mp3");
             var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
             var query = KnownFolders.MusicLibrary.CreateFileQueryWithOptions(queryOptions);
             IReadOnlyList<StorageFile> fileList = await query.GetFilesAsync();
             ReadMusicFiles(fileList);
             */
        }

        private void Songs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LocalSongs.Current.SongListLV.ItemsSource = Songs;
        }

        // 读取音乐文件的信息
        public async void ReadMusicFiles(IReadOnlyList<StorageFile> fileList)
        {
            foreach (StorageFile file in fileList)
            {
                if (!HasSong(file.Path))
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(file.Name, file);
                    MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                    StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView);
                    Songs.Add(new Song(file.Path, musicProperties, thumbnail));
                }
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

        public bool HasSong(string path)
        {
            foreach (Song song in Songs)
            {
                if (path == song.FilePath)
                {
                    return true;
                }
            }
            return false;
        }
    }
}