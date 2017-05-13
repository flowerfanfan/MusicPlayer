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
    public class FavoriteVM
    {
        private static FavoriteVM favoriteVM;
        public ObservableCollection<Song> FavoriteSongs { get; set; }
        public DataBaseManager DBManager { get; set; }

        private FavoriteVM()
        {
            DBManager = DataBaseManager.GetDBManager();
            LoadFavoriteSongs();
        }

        public static FavoriteVM GetFavoriteVM()
        {
            if (favoriteVM == null)
            {
                favoriteVM = new FavoriteVM();
            }
            return favoriteVM;
        }

        public void LoadFavoriteSongs()
        {
            FavoriteSongs = DBManager.GetSongs("_FavoriteSongs_");
        }

        public void AddFavoriteSong(Song song)
        {
            FavoriteSongs.Add(song);
            DBManager.AddSong(song, "_FavoriteSongs_");
        }

        public bool NoSuchSong(Song song)
        {
            foreach (Song item in FavoriteSongs)
            {
                if (song.Title == item.Title)
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveFavoriteSong(Song song)
        {
            FavoriteSongs.Remove(song);
            DBManager.DeleteSong("_FavoriteSongs_", song);
        }
    }
}
