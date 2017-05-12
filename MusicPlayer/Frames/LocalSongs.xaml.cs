using MusicPlayer.Controls;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MusicPlayer.Frames
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LocalSongs : Page
    {
        public LocalSongsVM localSongsVM { get; set; }
        private string EDIT = "编辑";
        private string CANCEL = "取消";
        private string ADD = "添加到歌单";

        public LocalSongs()
        {
            this.InitializeComponent();

            localSongsVM = new LocalSongsVM();
            // 加载本地歌曲
            localSongsVM.LoadSongs();
        }

        // 刷新按钮点击事件
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            localSongsVM.ReloadSongs();
        }

        // 编辑/取消/添加 按钮点击事件
        private async void AddSongsToListBtn_Click(object sender, RoutedEventArgs e)
        {
            // 编辑
            if (AddSongsToListBtn.Content.ToString() == EDIT)
            {
                AddSongsToListBtn.Content = CANCEL;
                SongListLV.SelectionMode = ListViewSelectionMode.Multiple;
            }
            // 取消
            else if (AddSongsToListBtn.Content.ToString() == CANCEL)
            {
                AddSongsToListBtn.Content = EDIT;
                SongListLV.SelectionMode = ListViewSelectionMode.Single;
            }
            // 添加
            else if (AddSongsToListBtn.Content.ToString() == ADD)
            {
                MySongListVM.GetMySongListVM().SongsToBeAddedToList = localSongsVM.SelectedSongs;
                // 弹出歌单选择对话框
                await new SelectSongListDialog().ShowAsync();
                AddSongsToListBtn.Content = EDIT;
                SongListLV.SelectionMode = ListViewSelectionMode.Single;
            }
        }

        // 选中或取消选中歌单事件处理
        private void SongListLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddSongsToListBtn.Content.ToString() != EDIT)
            {
                foreach (Song song in e.AddedItems)
                {
                    localSongsVM.SelectSong(song);
                }
                foreach (Song song in e.RemovedItems)
                {
                    localSongsVM.RemoveSelectedSong(song);
                }
                if (localSongsVM.HasSelected())
                {
                    AddSongsToListBtn.Content = ADD;
                }
                else
                {
                    AddSongsToListBtn.Content = CANCEL;
                }
            }
        }
    }
}