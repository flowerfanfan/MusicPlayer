using MusicPlayer.DataBase;
using MusicPlayer.Frames;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.ViewModels
{
    public class PlayingListVM
    {
        public ObservableCollection<Song> PlayingList { get; set; }

        public PlayingListVM()
        {
            PlayingList = DataBaseManager.GetDBManager().GetSongs("_PlayingList_");
            if (PlayingList.Count == 0)
            {
                PlayingList = LocalSongsVM.GetLocalSongsVM().Songs;
            }
        }

        public void SetPlayingList(ObservableCollection<Song> playingList)
        {
            PlayingList.Clear();
            foreach (Song song in playingList)
            {
                PlayingList.Add(song);
            }
        }
    }
}
