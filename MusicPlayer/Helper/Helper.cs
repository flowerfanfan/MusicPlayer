using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MediaPlayer
{
    static class Helper
    {
        static public bool isAudio(StorageFile file)
        {
            return (file.FileType != ".mp4" && file.FileType != ".mkv" && file.FileType != ".avi");
        }
    }
}
