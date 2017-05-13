using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public class SongList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        public string Name {
            get { return _name; }
            set {
                _name = value;
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }
        private int _number;
        public int Number {
            get { return _number; }
            set {
                _number = value;
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(nameof(Number)));
                }
            }
        }

        public SongList(string name, int number)
        {
            Name = name;
            Number = number;
        }

    }
}
