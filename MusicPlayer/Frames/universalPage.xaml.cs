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
    public sealed partial class universalPage : Page
    {
        public LocalSongsVM localSongsVM { get; set; }
        public universalPage()
        {
            this.InitializeComponent();
            localSongsVM = LocalSongsVM.GetLocalSongsVM();
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
         async void getRecentFiles()
        {
            AccessListEntryView entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries;
            //存放得到的recent歌曲
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
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
            LocalSongsVM.GetLocalSongsVM().Songs = songs;
            
        }


        /*
         下面的直接复制过来的， 从LocalSongs
         
         */

        private string EDIT = "编辑";
        private string CANCEL = "取消";
        private string ADD = "添加到歌单";
        private bool isSelecting;
        Song song;
        // 编辑/取消/添加 按钮点击事件
        private async void AddSongsToListBtn_Click(object sender, RoutedEventArgs e)
        {
            // 编辑
            if (AddSongsToListBtn.Content.ToString() == EDIT)
            {
                AddSongsToListBtn.Content = CANCEL;
                SongListLV.SelectionMode = ListViewSelectionMode.Multiple;
                SongListLV.IsItemClickEnabled = false;
                isSelecting = true;
            }
            // 取消
            else if (AddSongsToListBtn.Content.ToString() == CANCEL)
            {
                AddSongsToListBtn.Content = EDIT;
                SongListLV.SelectionMode = ListViewSelectionMode.Single;
                SongListLV.IsItemClickEnabled = true;
            }
            // 添加
            else if (AddSongsToListBtn.Content.ToString() == ADD)
            {
                MySongListVM.GetMySongListVM().SongsToBeAddedToList = localSongsVM.SelectedSongs;
                // 弹出歌单选择对话框
                await new SelectSongListDialog().ShowAsync();
                AddSongsToListBtn.Content = EDIT;
                SongListLV.SelectionMode = ListViewSelectionMode.Single;
                SongListLV.IsItemClickEnabled = true;
            }
        }

        // 选中或取消选中歌曲事件处理
        private void SongListLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddSongsToListBtn.Content.ToString() != EDIT)
            {
                if (isSelecting)
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
                if (localSongsVM.HasSelected())
                {
                    AddSongsToListBtn.Content = ADD;
                    DeleteSongsBtn.Opacity = 1;
                    DeleteSongsBtn.IsEnabled = true;
                }
                else
                {
                    AddSongsToListBtn.Content = CANCEL;
                    DeleteSongsBtn.IsEnabled = false;
                    DeleteSongsBtn.Opacity = 0;
                }
            }
        }

        // 删除选中的歌曲
        private void DeleteSongsBtn_Click(object sender, RoutedEventArgs e)
        {
            isSelecting = false;
            localSongsVM.DeleteSelectedSongs();
            AddSongsToListBtn.Content = EDIT;
            DeleteSongsBtn.IsEnabled = false;
            DeleteSongsBtn.Opacity = 0;
            SongListLV.SelectionMode = ListViewSelectionMode.Single;
            SongListLV.IsItemClickEnabled = true;
        }

        private void Select_Songs(object sender, ItemClickEventArgs e)
        {
            song = (Song)e.ClickedItem;

        }
        private async void PlaySong(object sender, DoubleTappedRoutedEventArgs e)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(song.FilePath);
            MainPage.Current.Play(file);
        }


        //To Finish
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            getRecentFiles();
        }
    }
}
