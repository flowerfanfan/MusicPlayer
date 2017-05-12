using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public class SongList
    {
        public string Name { get; set; }
        public int Number { get; set; }

        public SongList(string name, int number)
        {
            Name = name;
            Number = number;
        }
    }
}
