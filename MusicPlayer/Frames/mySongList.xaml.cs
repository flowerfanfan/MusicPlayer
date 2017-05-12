using MusicPlayer.Controls;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using System;
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
        private bool isSelceting;

        public mySongList()
        {
            this.InitializeComponent();

            mySongListVM = MySongListVM.GetMySongListVM();
        }

        // 点击歌单，展示歌曲列表
        private void SongListsLV_ItemClick(object sender, ItemClickEventArgs e)
        {
            mySongListVM.SetClickedList(((SongList)e.ClickedItem).Name);
        }

        // 编辑/取消/删除 按钮点击事件
        private async void DeleteListBtn_Click(object sender, RoutedEventArgs e)
        {
            // 编辑
            if (DeleteListBtn.Content.ToString() == EDIT)
            {
                // 还没有自建歌单时，弹出创建歌单对话框
                if (mySongListVM.NoListExist())
                {
                    await new CreateSongListDialog().ShowAsync();
                }
                else
                {
                    DeleteListBtn.Content = CANCEL;
                    SongListsLV.SelectionMode = ListViewSelectionMode.Multiple;
                    SongListsLV.IsItemClickEnabled = false;
                    isSelceting = true;
                }
            }
            // 取消
            else if (DeleteListBtn.Content.ToString() == CANCEL)
            {
                DeleteListBtn.Content = EDIT;
                SongListsLV.SelectionMode = ListViewSelectionMode.Single;
                SongListsLV.IsItemClickEnabled = true;
            }
            // 删除
            else if (DeleteListBtn.Content.ToString() == DELETE)
            {
                isSelceting = false;
                mySongListVM.DeleteSelectedLists();
                DeleteListBtn.Content = EDIT;
                SongListsLV.SelectionMode = ListViewSelectionMode.Single;
                SongListsLV.IsItemClickEnabled = true;
            }
        }

        // 选中或取消选中歌单事件处理
        private void SongListsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeleteListBtn.Content.ToString() != EDIT)
            {
                if (isSelceting)
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
                    DeleteListBtn.Content = DELETE;
                }
                else
                {
                    DeleteListBtn.Content = CANCEL;
                }
            }
        }
    }
}