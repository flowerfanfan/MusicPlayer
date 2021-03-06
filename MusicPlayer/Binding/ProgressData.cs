﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using MediaPlayer;
using Windows.UI.Xaml.Data;

namespace MusicPlayer.Binding
{
    public sealed class ProgressData : NotifyPropertyChangedBase
    {
        private double _max, _currvalue;
        public double Max
        {
            get { return _max; }
            set
            {
                if (value != _max)
                {
                    _max = value;
                    OnPropertyChanged();
                }
            }
        }


        public double CurrentValue
        {
            get { return _currvalue; }
            set
            {
                if (value != _currvalue)
                {
                    _currvalue = value;
                    OnPropertyChanged();

                }
            }
        }
    }

    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected async void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            });
        }
    }

    //动画用的
    public class MusicConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeSpan)value).TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return TimeSpan.FromSeconds((double)value);
        }
    }

    public class TimeConverter : IValueConverter
    {
        public object Convert(object v, Type targetType, object parameter, string language)
        {
            //简单省事不会错的方法。 就是对付圆周率之歌这些奇葩的时候会有点用
            //其他时候，浪费布局空间（小时不需要输出）
            double value = (double)v;
            if (value== 0.0) value = 0.1;
            string minutes = ((int)value / 60).ToString();
            if (((int)value) / 60 < 10) minutes = "0" + minutes;
            string seconds = (((int)value) % 60).ToString();
            if (((int)value) % 60 < 10) seconds = "0" + seconds;
            return minutes + ":" + seconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return TimeSpan.FromSeconds((double)value);
        }
    }
}