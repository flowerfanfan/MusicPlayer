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
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Navigation;
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
        public ObservableCollection<Song> PlayingList { get; set; }
        public Song ClickedSong { get; set; }
        public PlayingListVM playingListVM { get; set; }
        public ProgressData media = new ProgressData();
        public StorageFile mediaFile = null;
        public MainPage()
        {
            //设置页面为static， 以便于得到控件
            this.InitializeComponent();
            Current = this;
            App.Current.Suspending += Current_Suspending;
            player.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
            InitialColorSetting();

            InitialPlayerSetting();

            //指定页面
            ContentFrame.Navigate(typeof(Default));
            double t = ContentFrame.ActualWidth;

            DBManager = DataBaseManager.GetDBManager();
            playingListVM = new PlayingListVM();
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
                player.MediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace(file.Name, file);
                play_Click(player, null);
                mediaFile = file;
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
                    text = await FileIO.ReadTextAsync(lrcFile, UnicodeEncoding.Utf8);
                }
                lrc.getLrc(text);
                var properties = await file.Properties.GetMusicPropertiesAsync();
                Song s = new Song(file.Path, properties, thumbnail);
                s.lyric = lrc;
                // 更新磁贴
                TileManager.UpdateTileAsync(s);
                // 设置播放页面的 喜爱 按钮
                Default.Current.FavoriteBtnControl.Visibility = Visibility.Visible;
                if (FavoriteVM.GetFavoriteVM().NoSuchSong(s))
                {
                    Default.Current.FavoriteBtnControl.Source = Default.Current.Dislike;
                }
                else
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
        async Task TestSomethingAsync(MediaPlaybackSession sender)
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
                request.Data.Properties.Title = "分享音乐: " + mediaFile.DisplayName;
                request.Data.Properties.Description = "这是我喜欢听的音乐， 分享给你们，一起听一下吧~\n";
                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(mediaFile);
                List<IStorageItem> musicList = new List<IStorageItem>();
                musicList.Add(mediaFile as IStorageItem);
                request.Data.SetStorageItems(musicList);
                var ymd = DateTime.Now;
                request.Data.SetText("这是我喜欢听的音乐， 分享给你们，一起听一下吧~\n" + ymd.Year.ToString() + "年" + ymd.Month.ToString() + "月" + ymd.Day.ToString() + "日\n");

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
                Current.Frame.IsEnabled = false;
                var t = new MessageDialog("请等待...");
                await t.ShowAsync();
                var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
                var query = folder.CreateFileQueryWithOptions(queryOptions);
                IReadOnlyList<StorageFile> fileList = await query.GetFilesAsync();
                LocalSongsVM.GetLocalSongsVM().ReadMusicFiles(fileList);

                Current.Frame.IsEnabled = true;

                new MessageDialog("歌曲导入完成。");
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
            double newVolume = e.NewValue / 100;
            player.MediaPlayer.Volume = newVolume; //binding its? 
            if (newVolume == 0)
            {
                VolumeButton.Icon = new FontIcon
                {
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\uE992"
                };
            }
            else if (newVolume > 0 && newVolume <= 0.33)
            {
                VolumeButton.Icon = new FontIcon
                {
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\uE993"
                };
            }
            else if (newVolume > 0.33 && newVolume <= 0.66)
            {
                VolumeButton.Icon = new FontIcon
                {
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\uE994"
                };
            }
            else
            {
                VolumeButton.Icon = new FontIcon
                {
                    FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\uE995"
                };
            }
        }

        /*
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
        }*/

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ContentFrame.Navigate(typeof(SearchResults), args.QueryText.ToString());
        }

        private void previous_Click(object sender, RoutedEventArgs e)
        {
            if (Default.Current.song == null) return;
            var list = playingListVM.PlayingList;
            int k = list.getIndexOf(Default.Current.song);
            int previous = (k - 1 + list.Count) % list.Count;
            PlaySongAt(previous);
        }

        private async void PlaySongAt(int k)
        {
            var list = playingListVM.PlayingList;
            mediaFile = await StorageFile.GetFileFromPathAsync(list.ElementAt(k).FilePath);
            Play(mediaFile);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (Default.Current.song == null) return;
            var list = playingListVM.PlayingList;
            int k = list.getIndexOf(Default.Current.song);
            int next = (k + 1) % list.Count;
            PlaySongAt(next);
        }

        private void Select_Songs(object sender, ItemClickEventArgs e)
        {
            ClickedSong = (Song)e.ClickedItem;
        }

        private async void PlaySong(object sender, DoubleTappedRoutedEventArgs e)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(ClickedSong.FilePath);
            Play(file);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            /*if(((App)App.Current).IsSuspend)*/
            {
                var composite = new ApplicationDataCompositeValue();
                composite["PlayingList"] = playingListVM.PlayingList;
                composite["PlayingSong"] = mediaFile;
                ApplicationData.Current.LocalSettings.Values["MainPageData"] = composite;
                //Date need to complete
            }
        }


        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            {
                var composite = new ApplicationDataCompositeValue();
                //composite["PlayingList"] = playingListVM.PlayingList as Object;
                if (mediaFile != null)
                {
                    composite["PlayingSong"] = mediaFile.Path;
                    ApplicationData.Current.LocalSettings.Values["MainPageData"] = composite;
                }
                
                //Date need to complete
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MainPageData"))
            {
                var composite = ApplicationData.Current.LocalSettings.Values["MainPageData"] as ApplicationDataCompositeValue;
                //if (composite.ContainsKey("PlayingList")) playingListVM.PlayingList = (ObservableCollection<Song>)composite["PlayingList"];
                if (composite.ContainsKey("PlayingSong"))
                {
                    string filePath = (string)composite["PlayingSong"];
                    mediaFile = await StorageFile.GetFileFromPathAsync(filePath);
                    player.MediaPlayer.Source = MediaSource.CreateFromStorageFile(mediaFile);
                    /* while (player.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)*/
                    Song s = await mediaFile.ToSong();
                    ContentFrame.Navigate(typeof(Default), s);


                    Default.Current.FavoriteBtnControl.Visibility = Visibility.Visible;
                    if (FavoriteVM.GetFavoriteVM().NoSuchSong(s))
                    {
                        Default.Current.FavoriteBtnControl.Source = Default.Current.Dislike;
                    }
                    else
                    {
                        Default.Current.FavoriteBtnControl.Source = Default.Current.Like;
                    }
                }
            }
            ApplicationData.Current.LocalSettings.Values.Remove("MainPageData");
        }
    }
}
