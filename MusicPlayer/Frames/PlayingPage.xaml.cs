using MediaPlayer;
using MusicPlayer.Models;
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
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MusicPlayer.Frames
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlayingPage : Page
    {
        //这种用static的方法或许能帮助避免多次生成不同页面的方法， 但是我暂时还没去想。
        public static PlayingPage Current;
        DispatcherTimer timer;
        //这里我单独写了一个lyric变量不太好...之前没有在Song里面加入lyric结构， 现在加上之后有些重复了。
        //但是代码用lrc用得比较多， 懒得改回来了。 之后再说
        //不影响使用。 只影响代码阅读. lrc和song里面的成员lyric是一致的
        Lyric lrc = new Lyric();
        MediaPlayerElement player = MainPage.Current.player;
        public Song song;
        static bool Isinit = false;
        public PlayingPage()
        {
            DataContext = song;
            DataContextChanged += PlayingPage_DataContextChanged;
            if (Current == null)
            {
                this.InitializeComponent();
                Current = this;
            }


        }

        private void PlayingPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            /*
            songTitle.Text = song.Title;
            songArtist.Text = song.Artist;
            songAlbum.Text = song.Album;
            */
            if (song != null)
            {
                MainPage.Current.SongArtist.Text = song.Artist;
                MainPage.Current.SongTitle.Text = song.Title;
                MainPage.Current.Cover.Source = song.Cover;
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
                /*
                tn_pic.ImageSource = song.Cover;*/
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
            /*
            songTitle.Text = song.Title;
            songAlbum.Text = "Album: " + song.Album;
            songArtist.Text = "Artist: " + song.Artist;*/
            lrcText.Text = lrc.getAllText();
            lrcText.LineStackingStrategy = LineStackingStrategy.BaselineToBaseline;
            lrcText.LineHeight = Parameter.lyricLineHeight;
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
                
                //lyricBlock.Text = sentence;
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
            lrcContainer.ChangeView(null, offset, null, false);
        }

    }
}
