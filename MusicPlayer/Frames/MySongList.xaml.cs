using MusicPlayer.ViewModels;
using MusicPlayer.Frames;
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

namespace MusicPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MySongList : Page
    {
        public LocalSongsVM localSongsVM { get; set; }

        public MySongList()
        {
            this.InitializeComponent();

            localSongsVM = new LocalSongsVM();
            localSongsVM.LoadSongs();
        }

        private void SongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ListBoxItem)((ListBox)sender).SelectedItem).Name)
            {
                case "LocalSongsItem":
                    SongListFrame.Navigate(typeof(LocalSongs));
                    break;
                default:
                    break;
            }
        }
    }
}
