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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace MusicPlayer.Controls
{
    public sealed partial class SelectSongListDialog : ContentDialog
    {
        public ObservableCollection<SongList> MySongLists { get; set; }
        public string SelectedSongList { get; set; }

        public SelectSongListDialog()
        {
            this.InitializeComponent();
            MySongLists = MySongListVM.GetMySongListVM().SongLists;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MySongListVM.GetMySongListVM().AddSongsToList(((SongList)MySongListsLV.SelectedItem).Name);
        }

        private void MySongListsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                IsPrimaryButtonEnabled = true;
            }
        }
    }
}
