using MusicPlayer.Controls;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MusicPlayer.Frames
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LocalSongs : Page
    {
        public LocalSongsVM localSongsVM { get; set; }
        public LocalSongs()
        {
            this.InitializeComponent();

            localSongsVM = new LocalSongsVM();
            localSongsVM.LoadSongs();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            localSongsVM.ReloadSongs();
        }

        private async void AddSongsToList_Click(object sender, RoutedEventArgs e)
        {
            MySongListVM.GetMySongListVM().SongsToBeAddedToList = localSongsVM.SelectedSongs;
            // 弹出歌单选择对话框
            await new SelectSongListDialog().ShowAsync();
        }

        private void SongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Song song in e.AddedItems)
            {
                localSongsVM.SelectSong(song);
            }
            foreach (Song song in e.RemovedItems)
            {
                localSongsVM.RemoveSelectedSong(song);
            }
        }
    }
}
