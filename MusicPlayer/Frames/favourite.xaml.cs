using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicPlayer.Frames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class favourite : Page
    {
        public FavoriteVM favoriteVM { get; set; }
        public Song ClickedSong { get; set; }

        public favourite()
        {
            this.InitializeComponent();

            favoriteVM = FavoriteVM.GetFavoriteVM();
        }

        private void UnFavoriteBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            favoriteVM.RemoveFavoriteSong(((Song)((Image)sender).DataContext));
        }

        private void FavoriteGV_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClickedSong = (Song)e.ClickedItem;
        }

        private async void PlaySong(object sender, DoubleTappedRoutedEventArgs e)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(ClickedSong.FilePath);
            MainPage.Current.Play(file);
        }
    }
}
