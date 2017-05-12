using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
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
    public sealed partial class mySongList : Page
    {
        public MySongListVM mySongListVM { get; set; }

        public mySongList()
        {
            this.InitializeComponent();

            mySongListVM = MySongListVM.GetMySongListVM();
        }

        private void SongLists_ItemClick(object sender, ItemClickEventArgs e)
        {
            mySongListVM.SetSelectedList(((SongList)e.ClickedItem).Name);
        }

        private void DeleteSongListBtn_Click(object sender, RoutedEventArgs e)
        {
            // 应弹出对话框向用户确定
            mySongListVM.DeleteSongList(((SongList)((AppBarButton)sender).DataContext).Name);
        }

        private void DeleteSongListBtn_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((AppBarButton)sender).Opacity = 1;
        }

        private void DeleteSongListBtn_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((AppBarButton)sender).Opacity = 0;
        }
    }
}
