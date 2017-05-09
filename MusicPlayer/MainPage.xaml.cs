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

            ContentFrame.Navigate(typeof(Default));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            // 点击HamburgerButton时将MenuList的IsPaneOpen属性值设为相反的值
            MenuList.IsPaneOpen = !MenuList.IsPaneOpen;
        }

        private void IconListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ListBoxItem)((ListBox)sender).SelectedItem).Name)
            {
                case "SearchItem":
                    MenuList.IsPaneOpen = true;
                    break;
                case "SongListItem":
                    ContentFrame.Navigate(typeof(MySongList));
                    break;
            }
        }
    }
}
