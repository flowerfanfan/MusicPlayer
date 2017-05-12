using MusicPlayer.DataBase;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MusicPlayer.ViewModels
{
    public class MySongListVM
    {
        // 因需要与其他页面（MainPage）进行数据交换，故采用单例模式
        private static MySongListVM mySongListVM;
        public DataBaseManager DBManager { get; set; }
        public ObservableCollection<SongList> SongLists { get; set; }
        public Dictionary<string, ObservableCollection<Song>> SongsInList { get; set; }
        public ObservableCollection<Song> SongsInSelectedList { get; set; }
        public string ClickedListName { get; set; }
        public ObservableCollection<Song> SongsToBeAddedToList { get; set; }
        public ObservableCollection<SongList> SelectedLists { get; set; }

        private MySongListVM()
        {
            DBManager = DataBaseManager.GetDBManager();
            SongLists = new ObservableCollection<SongList>();
            SongsInList = new Dictionary<string, ObservableCollection<Song>>();
            SongsInSelectedList = new ObservableCollection<Song>();
            SongsToBeAddedToList = new ObservableCollection<Song>();
            SelectedLists = new ObservableCollection<SongList>();
            LoadSongLists();
        }

        // 从数据库加载歌单及歌单内的歌曲信息
        public void LoadSongLists()
        {
            DBManager.GetSongLists(SongLists, SongsInList);
        }

        // 获得MySongListVM实例
        public static MySongListVM GetMySongListVM()
        {
            if (mySongListVM == null)
            {
                mySongListVM = new MySongListVM();
            }
            return mySongListVM;
        }

        // 判断是否已有歌单
        public bool NoListExist()
        {
            return SongLists.Count == 0;
        }

        // 新建歌单
        public void CreateSongList(string listName)
        {
            if (IsNameValid(listName))
            {
                SongsInList.Add(listName, new ObservableCollection<Song>());
                SongLists.Add(new SongList(listName, 0));
                // 在数据库中对SongLists插入新纪录，创建新表
                DBManager.CreateSongList(listName);
            }
            else
            {
                // 提示用户输入的歌单名不合法或重复
            }
        }

        private bool IsNameValid(string listName)
        {/*
            for (int i = 0; i < 9; i++)
            {
                if (listName[0].ToString() == i.ToString())
                {
                    return false;
                }
            }*/
            return !SongsInList.ContainsKey(listName);
        }

        // 将一批歌曲加入歌单
        public void AddSongsToList(string name)
        {
            ObservableCollection<Song> songsInList = SongsInList[name];
            foreach (Song song in SongsToBeAddedToList)
            {
                songsInList.Add(song);
            }
            foreach (SongList songList in SongLists)
            {
                if (songList.Name == name)
                {
                    songList.Number = songsInList.Count;
                }
            }
            // 在数据库中添加
            DBManager.AddSongs(SongsToBeAddedToList, name);
        }

        // 用户点击一个歌单后，设置展示数据源
        public void SetClickedList(string name)
        {
            SongsInSelectedList.Clear();
            foreach (Song song in SongsInList[name])
            {
                SongsInSelectedList.Add(song);
            }
            ClickedListName = name;
        }

        // 批量删除选中的歌单
        public void DeleteSelectedLists()
        {
            foreach (SongList songList in SelectedLists)
            {
                DeleteSongList(songList);
            }
            SelectedLists.Clear();
        }

        // 删除一个歌单
        public void DeleteSongList(SongList songList)
        {
            SongsInList.Remove(songList.Name);
            SongLists.Remove(songList);
            DBManager.DeleteSongList(songList.Name);
            if (ClickedListName == songList.Name)
            {
                mySongListVM.SongsInSelectedList.Clear();
            }
        }

        public bool HasSelected()
        {
            return SelectedLists.Count != 0;
        }

        public void SelectList(SongList songList)
        {
            SelectedLists.Add(songList);
        }

        public void RemoveSelectedList(SongList songList)
        {
            SelectedLists.Remove(songList);
        }
    }
}
