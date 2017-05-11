using MusicPlayer.DataBase;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace MusicPlayer.Controls
{
    public sealed partial class CreateSongListDialog : ContentDialog
    {
        public CreateSongListDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 在数据库中对SongList插入新纪录，创建新表
            DataBaseManager.GetDBManager().CreateSongList(SongListName.Text.ToString());
            // vm
        }

        private void SongListName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SongListName.Text.ToString().Equals(""))
            {
                IsPrimaryButtonEnabled = false;
            }
            else
            {
                IsPrimaryButtonEnabled = true;
            }
        }
    }
}
