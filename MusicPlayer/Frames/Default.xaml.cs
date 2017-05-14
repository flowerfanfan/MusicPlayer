using MediaPlayer;
using MusicPlayer.Helper;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MusicPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Default : Page
    {
        //这种用static的方法或许能帮助避免多次生成不同页面的方法， 但是我暂时还没去想。
        public static Default Current;
        DispatcherTimer timer;
        Lyric lrc = new Lyric();
        MediaPlayerElement player = MainPage.Current.player;
        public Song song;
        static bool Isinit = false;
        public Image FavoriteBtnControl { get; set; }
        public BitmapImage Like { get; set; }
        public BitmapImage Dislike { get; set; }


        public Default()
        {
            DataContext = song;
            DataContextChanged += PlayingPage_DataContextChanged;
            if (Current == null)
            {
                this.InitializeComponent();
                Current = this;
                FavoriteBtnControl = FavoriteBtn;
                Like = new BitmapImage(new Uri("ms-appx:///Assets/like.png"));
                Dislike = new BitmapImage(new Uri("ms-appx:///Assets/dislike.png"));
            }
        }

        private void PlayingPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (song != null)
            {
                MainPage.Current.SongArtist.Text = song.Artist;
                MainPage.Current.SongTitle.Text = song.Title;
                MainPage.Current.Cover.Source = song.Cover;
                search.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //这句不知道什么意思， 看上去好像是设置重载函数的样子
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                song = (Song)e.Parameter;
                lrc = song.lyric;
                setLRC();
                if (!Isinit)
                {
                    InitRotate();
                    Isinit = true;
                }
                DataContext = song;
            }
        }
        void setLRC()
        {
            lrcText.Text = lrc.getAllText();
            lrcText.LineStackingStrategy = LineStackingStrategy.BaselineToBaseline;
            lrcText.LineHeight = Parameter.lyricLineHeight;
            lrcText.VerticalAlignment = VerticalAlignment.Top;
        }
        private void InitRotate()
        {
            timer = new DispatcherTimer();
            rotation.CenterY += ellipse.Height / 2;
            rotation.CenterX += ellipse.Width / 2;
            out_rotation.CenterY += ring.Height / 2;
            out_rotation.CenterX += ring.Width / 2;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Tick += Rotate;
            timer.Tick += changeLyric;
            timer.Start();
        }

        private void Rotate(object sender, object e)
        {
            if (player.MediaPlayer.PlaybackSession.PlaybackState == Windows.Media.Playback.MediaPlaybackState.Playing)
            {
                rotation.Angle += 0.5;
                out_rotation.Angle += 0.25;
            }
        }

        private void changeLyric(object sender, object e)
        {
            var session = player.MediaPlayer.PlaybackSession;
            int previousIndex = lrc.currentIndex;
            if (session.PlaybackState == MediaPlaybackState.Playing)
            {
                string sentence = lrc.getCurrentSentence(session.Position.TotalSeconds);
                //lyric跳转了的时候
                int lineNum = sentence.Split('\n').Length - 1;
                if (lrc.currentIndex != previousIndex)
                    adjustScrollOffset(lrc.getLyricLines());
            }
        }
        void adjustScrollOffset(double k)
        {
            double staticlength = lrcContainer.ActualHeight / 2 + Parameter.lyricLineHeight;
            double offset = Parameter.lyricLineHeight * k;
            if (offset < staticlength)
            {
                offset = 0;
            }
            else
            {
                offset -= staticlength;
            }
            lrcContainer.ChangeView(null, offset + Parameter.Offset, null, false);
        }

        private void SearchLRCOnline(object sender, RoutedEventArgs e)
        {
            string title = song.Title;
            string lyric = WebRequest.getLyric(title);
            if (lyric == "No lyric")
            {
                Current.lrcText.Text = "暂无歌词";
                Current.lrcText.VerticalAlignment = VerticalAlignment.Center;
            }
            else
            {
                lrc = new Lyric(lyric);
                setLRC();
            }
        }

        private void FavoriteBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (song != null)
            {
                if (((BitmapImage)FavoriteBtn.Source).UriSource == Dislike.UriSource)
                {
                    FavoriteVM.GetFavoriteVM().AddFavoriteSong(song);
                    FavoriteBtn.Source = Like;
                }
                else
                {
                    FavoriteVM.GetFavoriteVM().RemoveFavoriteSong(song);
                    FavoriteBtn.Source = Dislike;
                }
            }
        }

        private void LRCup_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Parameter.Offset += 10;
            var session = player.MediaPlayer.PlaybackSession;
            string sentence = lrc.getCurrentSentence(session.Position.TotalSeconds);
            //lyric跳转了的时候
            int lineNum = sentence.Split('\n').Length - 1;
            adjustScrollOffset(lrc.getLyricLines());
        }

        private void LRCdown_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Parameter.Offset -= 10;
            var session = player.MediaPlayer.PlaybackSession;
            string sentence = lrc.getCurrentSentence(session.Position.TotalSeconds);
            //lyric跳转了的时候
            int lineNum = sentence.Split('\n').Length - 1;
            adjustScrollOffset(lrc.getLyricLines());
        }
    }
}
