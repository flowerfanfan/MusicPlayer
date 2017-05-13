using MediaPlayer;
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
using Windows.Storage.AccessCache;
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
    public sealed partial class recent : Page
    {
        public recent()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            getRecentFiles();
        }


        async void getRecentFiles()
        {
            AccessListEntryView entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries;
            //存放得到的recent歌曲
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
            if (entries.Count > 0)
            {
                foreach (AccessListEntry entry in entries)
                {
                    StorageFile file =  await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(entry.Token);
                    Song s = await file.ToSong();
                    songs.Add(s);
                }
            }
            LocalSongsVM.GetLocalSongsVM().Songs = songs;

        }
    }
}
