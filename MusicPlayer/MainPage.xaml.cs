using MusicPlayer.Controls;
using MusicPlayer.Frames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace MusicPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            //SAY("Hello World!");
            
            this.InitializeComponent();
            //设置窗口栏的颜色
            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Crimson;
            titleBar.ForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Crimson;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Colors.LightCoral;

            var a = ContentFrame.ActualWidth;

            ContentFrame.Navigate(typeof(Default));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            // 点击HamburgerButton时将MenuList的IsPaneOpen属性值设为相反的值
            MenuList.IsPaneOpen = !MenuList.IsPaneOpen;
        }

        private void IconListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 使用 ContentFrame.Navigate(typeof(Page)); 来加载页面
            if (SearchItem.IsSelected)
            {
                MenuList.IsPaneOpen = true;
            } else if (RecentItem.IsSelected)
            {
                ContentFrame.Navigate(typeof(recent));
                RecentItem.IsSelected = false;
            } else if (FavoriteItem.IsSelected)
            {
                ContentFrame.Navigate(typeof(favourite));
                FavoriteItem.IsSelected = false;
            } else if (ListItem.IsSelected)
            {
                ContentFrame.Navigate(typeof(LocalSongs));
                ListItem.IsSelected = false;
            } else if (mySongListItem.IsSelected)
            {
                ContentFrame.Navigate(typeof(mySongList));
                mySongListItem.IsSelected = false;
            }
        }

        void palyingNow(object sender, TappedRoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Default));
        }

        private async void AddSongListBtn_Click(object sender, RoutedEventArgs e)
        {
            mySongListItem.IsSelected = true;
            await new CreateSongListDialog().ShowAsync();
        }
    }
}
