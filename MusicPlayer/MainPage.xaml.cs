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
            ContentFrame.Navigate(typeof(PlayingPage));
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

            //a在这里干嘛的呢？ 加个注释呗~
            var a = ContentFrame.ActualWidth;

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
         不是很懂这个函数名称的含义...
         开始播放不应该是这里吧。。。我认为这里只有跳转
         如果不同理解的话这里注释一下... 没有的话可以改个函数名...
             */
        void playingNow(object sender, TappedRoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(PlayingPage));
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
            volume.Value = 0.5;
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


        private async void pickFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario
            //NotifyUser("", NotifyType.StatusMessage);

            // Create and open the file picker
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mkv");
            openPicker.FileTypeFilter.Add(".avi");
            openPicker.FileTypeFilter.Add(".mp3");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {

                var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.MusicView);
                BitmapImage tn = new BitmapImage();
                tn.SetSource(thumbnail);
                player.MediaPlayer.Source = MediaSource.CreateFromStorageFile(file);

                mediaFile = file;

                player.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;

                //
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(file.Name, file);
                string lrcPath = file.Path.Replace(".mp3", ".lrc");
                StorageFile lrcFile = null;
                var properties = await file.Properties.GetMusicPropertiesAsync();
                try
                {
                    lrcFile = await StorageFile.GetFileFromPathAsync(lrcPath);
                }
                catch (UnauthorizedAccessException ex)
                {

                    await new MessageDialog(ex.Message).ShowAsync();
                    /*
                    StorageFolder parent = null;
                    StorageFile lrcFile = null;
                    parent = await file.GetParentAsync();
                    while (parent == null) ; //在这里无限等待了
                    lrcFile = await parent.GetFileAsync(file.DisplayName + ".lrc");
                    */

                    openPicker = new FileOpenPicker();
                    openPicker.ViewMode = PickerViewMode.Thumbnail;
                    openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    openPicker.FileTypeFilter.Add(".lrc");
                    lrcFile = await openPicker.PickSingleFileAsync();
                    if (lrcFile != null)
                        StorageApplicationPermissions.FutureAccessList.AddOrReplace(lrcFile.Name, lrcFile);
                }
                //

                //while (lrcFile == null) ;
                string text = "";
                if (lrcFile != null)
                {

                    text = await Windows.Storage.FileIO.ReadTextAsync(lrcFile, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                }



                lrc.getLrc(text);
                Song s = new Song(file.Path, properties, thumbnail);
                s.lyric = lrc;
                s.Cover = tn;

                /*
                s.Album = properties.Album;
                s.Artist = properties.Artist;
                s.Cover = tn;
                s.Title = properties.Title;
                s.lyric = lrc;
                */
                ContentFrame.Navigate(typeof(PlayingPage), s);

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
                PlayingPage.Current.switchPauseAnimation.Begin();
            player.MediaPlayer.PlaybackSession.Position = new TimeSpan(0);
            player.MediaPlayer.Pause();
            PlayingPage.Current.rotation.Angle = 0;
            PlayingPage.Current.out_rotation.Angle = 0;

        }

        private void timelineChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            player.MediaPlayer.PlaybackSession.PositionChanged -= PlaybackSession_PositionChanged;
            if (Math.Abs(player.MediaPlayer.PlaybackSession.Position.TotalSeconds - timeline.Value) > 1)
                player.MediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds((double)timeline.Value);
            player.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
            if (Math.Abs(timeline.Value - media.Max) < 0.1) stop_Click(null, null);
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (player.MediaPlayer.Source != null && player.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
                PlayingPage.Current.switchOnAnimation.Begin();
            player.MediaPlayer.PlaybackSession.PlaybackRate = 1;
            player.MediaPlayer.Play();
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            if (player.MediaPlayer.Source != null && player.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Paused)
                PlayingPage.Current.switchPauseAnimation.Begin();

            player.MediaPlayer.Pause();
        }


        /*下面代码用于控制分享*/
        private async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            var deferral = args.Request.GetDeferral();

            try
            {
                //Uri musicUri = new Uri(mediaUri);
                //Uri pictureUri = ((BitmapImage)ViewModel.SelectedItem.image).UriSource;
                /*if (musicUri != null )*/
                /*StorageFile photoFile = null;
                 if (mediaUri != null)
                    photoFile = await StorageFile.GetFileFromApplicationUriAsync(mediaUri);*/

                request.Data.Properties.Title = mediaFile.DisplayName;
                request.Data.Properties.Description = "这是我喜欢听的音乐， 分享给你们，一起听一下吧~（这台词可以改）";

                // It's recommended to use both SetBitmap and SetStorageItems for sharing a single image
                // since the target app may only support one or the other.


                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(mediaFile);
                // It is recommended that you always add a thumbnail image any time you're sharing an image
                /*request.Data.Properties.Thumbnail = imageStreamRef;
                request.Data.SetBitmap(imageStreamRef);*/
                List<IStorageItem> musicList = new List<IStorageItem>();
                musicList.Add(mediaFile as IStorageItem);
                request.Data.SetStorageItems(musicList);
                // Set Text to share for those targets that can't accept images
                var ymd = DateTime.Now;
                request.Data.SetText("这是我喜欢听的音乐， 分享给你们，一起听一下吧~（这台词可以改）" + ymd.Year.ToString() + "年" + ymd.Month.ToString() + "月" + ymd.Day.ToString() + "日");

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

        // TODO
        private void AddSongBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
