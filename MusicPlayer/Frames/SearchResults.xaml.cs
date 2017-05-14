using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class SearchResults : Page
    {
        public ObservableCollection<Song> ResultSongs = new ObservableCollection<Song>();
        public Song ClickedSong { get; set; }

        public SearchResults()
        {
            this.InitializeComponent();
            ResultsLV.DataContextChanged += ResultsLV_DataContextChanged;
        }

        private void ResultsLV_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            ResultsLV.ItemsSource = ResultSongs;
        }

        private void ResultsLV_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClickedSong = (Song)e.ClickedItem;
        }

        private async void PlaySong(object sender, DoubleTappedRoutedEventArgs e)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(ClickedSong.FilePath);
            MainPage.Current.Play(file);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string searchContent = (string)e.Parameter;
            ResultSongs = LocalSongsVM.GetLocalSongsVM().SearchSongs(searchContent);
            if (ResultSongs.Count != 0)
            {
                NoResultTB.Visibility = Visibility.Collapsed;
                ResultsLV.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultTB.Visibility = Visibility.Visible;
                ResultsLV.Visibility = Visibility.Collapsed;
            }
        }
    }
}
