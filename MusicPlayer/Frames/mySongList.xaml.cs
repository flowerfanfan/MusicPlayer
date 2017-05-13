using MusicPlayer.Controls;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicPlayer.Frames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class mySongList : Page
    {
        public MySongListVM mySongListVM { get; set; }
        private string EDIT = "编辑";
        private string CANCEL = "取消";
        private string DELETE = "删除";
        private bool isSelcetingLists;
        private bool isSelectingSongs;
        private Song song;

        public mySongList()
        {
            this.InitializeComponent();

            mySongListVM = MySongListVM.GetMySongListVM();
            mySongListVM.SongsInClickedList.Clear();
        }

        // 点击歌单，展示歌曲列表
        private void SongListsLV_ItemClick(object sender, ItemClickEventArgs e)
        {
            string clickedListName = ((SongList)e.ClickedItem).Name;
            mySongListVM.SetClickedList(clickedListName);
            ClickedListNameTB.Text = clickedListName;
            ClickedListHead.Visibility = Visibility.Visible;
            MySongListVM.GetMySongListVM().PlayingList = MySongListVM.GetMySongListVM().SongsInList[clickedListName];
        }

        // 编辑/取消/删除 歌单按钮点击事件
        private async void DeleteListsBtn_Click(object sender, RoutedEventArgs e)
        {
            // 编辑
            if (DeleteListsBtn.Content.ToString() == EDIT)
            {
                // 还没有自建歌单时，弹出创建歌单对话框
                if (mySongListVM.NoListExist())
                {
                    await new CreateSongListDialog().ShowAsync();
                }
                else
                {
                    DeleteListsBtn.Content = CANCEL;
                    SongListsLV.SelectionMode = ListViewSelectionMode.Multiple;
                    SongListsLV.IsItemClickEnabled = false;
                    isSelcetingLists = true;
                }
            }
            // 取消
            else if (DeleteListsBtn.Content.ToString() == CANCEL)
            {
                DeleteListsBtn.Content = EDIT;
                SongListsLV.SelectionMode = ListViewSelectionMode.Single;
                SongListsLV.IsItemClickEnabled = true;
            }
            // 删除
            else if (DeleteListsBtn.Content.ToString() == DELETE)
            {
                isSelcetingLists = false;
                mySongListVM.DeleteSelectedLists();
                DeleteListsBtn.Content = EDIT;
                SongListsLV.SelectionMode = ListViewSelectionMode.Single;
                SongListsLV.IsItemClickEnabled = true;
            }
        }

        // 选中或取消选中歌单事件处理
        private void SongListsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeleteListsBtn.Content.ToString() != EDIT)
            {
                if (isSelcetingLists)
                {
                    foreach (SongList songList in e.AddedItems)
                    {
                        mySongListVM.SelectList(songList);
                    }
                    foreach (SongList songList in e.RemovedItems)
                    {
                        mySongListVM.RemoveSelectedList(songList);
                    }
                }
                if (mySongListVM.HasSelected())
                {
                    DeleteListsBtn.Content = DELETE;
                }
                else
                {
                    DeleteListsBtn.Content = CANCEL;
                }
            }
        }

        // 编辑/取消/删除 歌曲按钮点击事件
        private void DeleteSongsBtn_Click(object sender, RoutedEventArgs e)
        {
            // 编辑
            if (DeleteSongsBtn.Content.ToString() == EDIT)
            {
                DeleteSongsBtn.Content = CANCEL;
                SongsInSelectedListLV.SelectionMode = ListViewSelectionMode.Multiple;
                SongsInSelectedListLV.IsItemClickEnabled = false;
                isSelectingSongs = true;
            }
            // 取消
            else if (DeleteSongsBtn.Content.ToString() == CANCEL)
            {
                DeleteSongsBtn.Content = EDIT;
                SongsInSelectedListLV.SelectionMode = ListViewSelectionMode.Single;
                SongsInSelectedListLV.IsItemClickEnabled = true;
            }
            // 删除
            else if (DeleteSongsBtn.Content.ToString() == DELETE)
            {
                isSelectingSongs = false;
                mySongListVM.DeleteSelectedSongs();
                DeleteSongsBtn.Content = EDIT;
                SongsInSelectedListLV.SelectionMode = ListViewSelectionMode.Single;
                SongsInSelectedListLV.IsItemClickEnabled = true;
            }
        }

        // 选中或取消选中歌曲事件处理
        private void SongsInSelectedListLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeleteSongsBtn.Content.ToString() != EDIT)
            {
                if (isSelectingSongs)
                {
                    foreach (Song song in e.AddedItems)
                    {
                        mySongListVM.SelectSong(song);
                    }
                    foreach (Song song in e.RemovedItems)
                    {
                        mySongListVM.RemoveSelectedSong(song);
                    }
                }
                if (mySongListVM.HasSelectedSongs())
                {
                    DeleteSongsBtn.Content = DELETE;
                }
                else
                {
                    DeleteSongsBtn.Content = CANCEL;
                }
            }
        }

        private void Select_Songs(object sender, ItemClickEventArgs e)
        {
            song = (Song)e.ClickedItem;
        }

        private async void PlaySong(object sender, DoubleTappedRoutedEventArgs e)
        {
            //Song t = (Song)e.OriginalSource;
            
            StorageFile file = await StorageFile.GetFileFromPathAsync(song.FilePath);
            MainPage.Current.Play(file);
        }
    }
}