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
using MusicPlayer.Controls;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using Windows.Storage.AccessCache;
using System.Collections.ObjectModel;
using Windows.Storage;
using MediaPlayer;
using Windows.UI.Popups;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MusicPlayer.Frames
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class recent : Page
    {
        public static recent Current;
        Song song;
        ObservableCollection<Song> songs = new ObservableCollection<Song>();
        public LocalSongsVM localSongsVM { get; set; }
        public recent()
        {
            DataContext = songs;
            DataContextChanged += Recent_DataContextChanged;
            this.InitializeComponent();
            //localSongsVM = LocalSongsVM.GetLocalSongsVM();
            Current = this;
        }

        private void Recent_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            SongListLV.ItemsSource = songs;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
                PageTitle.Text = (string)e.Parameter;
            //LocalSongsVM.GetLocalSongsVM().Songs.Clear();

            RefreshBtn_Click(null, null);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // LocalSongsVM.GetLocalSongsVM().Songs.Clear();
        }
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            getRecentFiles();
        }
        public async void getRecentFiles()
        {
            AccessListEntryView entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries;
            //存放得到的recent歌曲
            songs = new ObservableCollection<Song>();
            if (entries.Count > 0)
            {
                songs.Clear();
                foreach (AccessListEntry entry in entries)
                {
                    StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(entry.Token);
                    Song s = await file.ToSong();
                    songs.Add(s);
                }
            }
            //LocalSongsVM.GetLocalSongsVM().Songs = songs;

        }

        private void Select_Songs(object sender, ItemClickEventArgs e)
        {
            song = (Song)e.ClickedItem;

        }
        private async void PlaySong(object sender, DoubleTappedRoutedEventArgs e)
        {
            MySongListVM.GetMySongListVM().PlayingList = songs;
            StorageFile file = await StorageFile.GetFileFromPathAsync(song.FilePath);
            MainPage.Current.Play(file);
            
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            StorageApplicationPermissions.MostRecentlyUsedList.Clear();
            RefreshBtn_Click(sender, e);
        }
    }
}
