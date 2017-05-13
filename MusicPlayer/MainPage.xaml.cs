using MediaPlayer;
using MusicPlayer.Binding;
using MusicPlayer.Frames;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.AccessCache;
using MusicPlayer.Controls;
using MusicPlayer.DataBase;
using Windows.Storage.FileProperties;
using System.Linq;
using MusicPlayer.ViewModels;
using MusicPlayer.Helper;
using Windows.Storage.Search;
using MusicPlayer.Tile;
//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace MusicPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Lyric lrc = new Lyric();
        public static MainPage Current;
        public DataBaseManager DBManager { get; set; }

        public MainPage()
        {
            //SAY("Hello World!");
            //设置页面为static， 以便于得到控件
            this.InitializeComponent();
            Current = this;

            InitialColorSetting();

            InitialPlayerSetting();

            //指定页面
            ContentFrame.Navigate(typeof(Default));
            double t = ContentFrame.ActualWidth;

            DBManager = DataBaseManager.GetDBManager();
        }

        //设置窗口栏的颜色
        void InitialColorSetting()
        {
            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Crimson;
            titleBar.ForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Crimson;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Colors.LightCoral;

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
            }
            else if (RecentItem.IsSelected)
            {
                ContentFrame.Navigate(typeof(Frames.recent));
                RecentItem.IsSelected = false;
            }
            else if (FavoriteItem.IsSelected)
            {
                ContentFrame.Navigate(typeof(Frames.favourite));
                FavoriteItem.IsSelected = false;
            }
            else if (ListItem.IsSelected)
            {
                ContentFrame.Navigate(typeof(Frames.LocalSongs));
                ListItem.IsSelected = false;
            }
            else if (mySongListItem.IsSelected)
            {
                ContentFrame.Navigate(typeof(Frames.mySongList));
                mySongListItem.IsSelected = false;
            }
        }

        /*
          这里是下边栏的图片点击响应函数，点击图片就会跳转到正在播放界面
              */
        void playingNow(object sender, TappedRoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Default));
        }


        /*播放用到的代码*/
        /*这里数据绑定写得实在是烂， 迟早要重写*/
        /*鉴于我的FullScreen也写得很烂， 我就先不加了orz*/
        public ProgressData media = new ProgressData();
        public StorageFile mediaFile = null;

        async void NotifyUser(string msg)
        {
            await new MessageDialog(msg).ShowAsync();
        }

        void InitialPlayerSetting()
        {
            DataContext = media;
            player.MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            player.MediaPlayer.Volume = 0.5;
            volume.Value = 50;
            player.MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }

        async private void MediaPlayer_MediaFailed(Windows.Media.Playback.MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await new MessageDialog("不支持该视频或音频的播放！请重新选择文件。").ShowAsync();
            });
        }

        private void MediaPlayer_MediaOpened(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            media.Max = sender.PlaybackSession.NaturalDuration.TotalSeconds;
        }

        public async void Play(StorageFile file)
        {
            if (file != null)
            {
                bool fileFromPicker = true;
                var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.MusicView);
                BitmapImage tn = new BitmapImage();
                tn.SetSource(thumbnail);
                player.MediaPlayer.Source = MediaSource.CreateFromStorageFile(file);

                //MostRecentlyUsedList 添加。
               /* if (StorageApplicationPermissions.MostRecentlyUsedList.ContainsItem(file.Name))
                    StorageApplicationPermissions.MostRecentlyUsedList.Remove(file.Name);*/
                StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace(file.Name, file);
                //自动播放！
                player.MediaPlayer.Play();
                play_Click(player, null);

                
                mediaFile = file;
                player.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
                if (StorageApplicationPermissions.FutureAccessList.CheckAccess(file))
                    fileFromPicker = false;
                else
                {
                    fileFromPicker = true;
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(file.Name, file);
                }



                string lrcPath = file.Path.Replace(".mp3", ".lrc");
                StorageFile lrcFile = null;

                try
                {
                    lrcFile = await StorageFile.GetFileFromPathAsync(lrcPath);
                }
                catch (Exception ex)
                {
                    if (fileFromPicker)
                    {
                        await new MessageDialog(ex.Message + ", 请手动添加歌词文件， 或者直接按“取消”进行无歌词播放").ShowAsync();
                        /*
                        StorageFolder parent = null;
                        StorageFile lrcFile = null;
                        parent = await file.GetParentAsync();
                        while (parent == null) ; //在这里无限等待了
                        lrcFile = await parent.GetFileAsync(file.DisplayName + ".lrc");
                        */

                        FileOpenPicker openPicker = new FileOpenPicker();
                        openPicker.ViewMode = PickerViewMode.Thumbnail;
                        openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                        openPicker.FileTypeFilter.Add(".lrc");
                        lrcFile = await openPicker.PickSingleFileAsync();
                    }
                }
                string text = "";
                if (lrcFile != null)
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(lrcFile.Name, lrcFile);
                    text = await Windows.Storage.FileIO.ReadTextAsync(lrcFile, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                }
                lrc.getLrc(text);
                var properties = await file.Properties.GetMusicPropertiesAsync();
                Song s = new Song(file.Path, properties, thumbnail);
                s.lyric = lrc;
                s.Cover = tn;
                // 更新磁贴
                TileManager.UpdateTileAsync(s);
                // 设置播放页面的 喜爱 按钮
                Default.Current.FavoriteBtnControl.Visibility = Visibility.Visible;
                if (FavoriteVM.GetFavoriteVM().NoSuchSong(s))
                {
                    Default.Current.FavoriteBtnControl.Source = Default.Current.Dislike;
                } else
                {
                    Default.Current.FavoriteBtnControl.Source = Default.Current.Like;
                }
                ContentFrame.Navigate(typeof(Default), s);
            }
        }

        async private void PlaybackSession_PositionChanged(Windows.Media.Playback.MediaPlaybackSession sender, object args)
        {
            await TestSomethingAsync(sender);
        }
        async Task TestSomethingAsync(Windows.Media.Playback.MediaPlaybackSession sender)
        {
            media.CurrentValue = sender.Position.TotalSeconds;
            await Task.Delay(0);
        }

        //在MainPage中控制另一个页面的动画， 感觉不是很好。
        private void stop_Click(object sender, RoutedEventArgs e)
        {
            if (player.MediaPlayer.Source != null && player.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Paused)
                Default.Current.switchPauseAnimation.Begin();
            player.MediaPlayer.PlaybackSession.Position = new TimeSpan(0);
            player.MediaPlayer.Pause();
            Default.Current.rotation.Angle = 0;
            Default.Current.out_rotation.Angle = 0;
            PlayButton.Icon = new SymbolIcon(Symbol.Play);
        }

        private void timelineChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            player.MediaPlayer.PlaybackSession.PositionChanged -= PlaybackSession_PositionChanged;
            if (Math.Abs(player.MediaPlayer.PlaybackSession.Position.TotalSeconds - timeline.Value) > 1)
                player.MediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds((double)timeline.Value);
            player.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
            //if (Math.Abs(timeline.Value - media.Max) < 0.2) stop_Click(null, null);
        }

        /*把play和pause button合到一起了*/
        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (player.MediaPlayer.Source != null && player.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
            {
                Default.Current.switchOnAnimation.Begin();
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
                PlayButton.Label = "Pause";
                player.MediaPlayer.PlaybackSession.PlaybackRate = 1;
                player.MediaPlayer.Play();
            }
            else if (player.MediaPlayer.Source != null && player.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Paused)
            {
                Default.Current.switchPauseAnimation.Begin();
                PlayButton.Icon = new SymbolIcon(Symbol.Play);
                PlayButton.Label = "Play";
                player.MediaPlayer.Pause();
            }
        }


        /*下面代码用于控制分享*/
        private async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            var deferral = args.Request.GetDeferral();

            try
            {
                request.Data.Properties.Title = mediaFile.DisplayName;
                request.Data.Properties.Description = "这是我喜欢听的音乐， 分享给你们，一起听一下吧~（这台词可以改）";
                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(mediaFile);
                List<IStorageItem> musicList = new List<IStorageItem>();
                musicList.Add(mediaFile as IStorageItem);
                request.Data.SetStorageItems(musicList);
                var ymd = DateTime.Now;
                request.Data.SetText("这是我喜欢听的音乐， 分享给你们，一起听一下吧~（这台词可以改）\n" + ymd.Year.ToString() + "年" + ymd.Month.ToString() + "月" + ymd.Day.ToString() + "日");

            }
            catch (NullReferenceException ex)
            {
                await new MessageDialog("请先选择需要分享的一首歌呀").ShowAsync();
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private async void AddSongListBtn_Click(object sender, RoutedEventArgs e)
        {
            mySongListItem.IsSelected = true;
            await new CreateSongListDialog().ShowAsync();
        }

        private async void Addfolder_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker openPicker = new FolderPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".mp3");
            StorageFolder folder = await openPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
                var query = folder.CreateFileQueryWithOptions(queryOptions);
                IReadOnlyList<StorageFile> fileList = await query.GetFilesAsync();
                LocalSongsVM.GetLocalSongsVM().ReadMusicFiles(fileList);
            }
        }

        private async void AddSongBtn_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".mp3");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                Play(file);
                MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView);
                if (!LocalSongsVM.GetLocalSongsVM().HasSong(file.Path))
                {
                    Song song = new Song(file.Path, musicProperties, thumbnail);
                    LocalSongsVM.GetLocalSongsVM().Songs.Add(song);
                    DBManager.AddSong(song, "_Songs_");
                }
            }
        }
        private void volumeChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            player.MediaPlayer.Volume = e.NewValue / 100; //binding its?
        }


        private void SearchContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchContent.Text.ToString() != "")
            {
                SearchBtn.IsEnabled = true;
            }
            else
            {
                SearchBtn.IsEnabled = false;
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(SearchResults), SearchContent.Text.ToString());
        }
    }
}
