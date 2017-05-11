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
        public DataBaseManager DBManager { get; set; }
        public List<ObservableCollection<Song>> SongList { get; set; }

        public MySongListVM()
        {
            DBManager = DataBaseManager.GetDBManager();
        }
    }
}
