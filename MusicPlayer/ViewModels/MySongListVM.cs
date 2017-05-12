using MusicPlayer.DataBase;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.ViewModels
{
    public class MySongListVM
    {
        private static MySongListVM mySongListVM;
        public DataBaseManager DBManager { get; set; }
        public ObservableCollection<SongList> SongLists { get; set; }
        public Dictionary<string, ObservableCollection<Song>> SongsInList { get; set; }
        public ObservableCollection<Song> SongsInSelectedList { get; set; }

        private MySongListVM()
        {
            DBManager = DataBaseManager.GetDBManager();
            SongLists = new ObservableCollection<SongList>();
            SongsInList = new Dictionary<string, ObservableCollection<Song>>();
        }

        public static MySongListVM GetMySongListVM()
        {
            if (mySongListVM == null)
            {
                mySongListVM = new MySongListVM();
                mySongListVM.LoadSongLists();
            }
            return mySongListVM;
        }

        public void LoadSongLists()
        {
            DBManager.GetSongLists(SongLists, SongsInList);
        }

        public void SetSelectedList(string name)
        {
            SongsInSelectedList = SongsInList[name];
        }

        public void CreateSongList(string listName)
        {
            if (IsNameValid(listName) && !SongsInList.ContainsKey(listName))
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
        {
            for (int i = 0; i < 9; i++)
            {
                if (listName[0].ToString() == i.ToString())
                {
                    return false;
                }
            }
            return true;
        }

        public void DeleteSongList(string name)
        {
            SongsInList.Remove(name);
            foreach (SongList songList in SongLists)
            {
                if (songList.Name.Equals(name))
                {
                    SongLists.Remove(songList);
                    break;
                }
            }
            DBManager.DeleteSongList(name);
        }
    }
}
