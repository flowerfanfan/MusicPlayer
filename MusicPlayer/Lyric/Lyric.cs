using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MediaPlayer
{
    class Sentence
    {
        public TimeSpan time;
        public string content;
        public int index;

        public Sentence(TimeSpan ts, string text, int i)
        {
            this.time = ts;
            this.content = text;
            this.index = i;
        }
    }
    public class Lyric
    {
        Uri lrcPath;
        List<Sentence> lrc = new List<Sentence>();
        public int currentIndex;
        public Lyric()
        {
            //
            currentIndex = 0;
        }
        public Lyric(string text)
        {
            getLrc(text);
        }
        public string getAllText()
        {
            List<string> result = new List<string>();
            string re = "";
            foreach (Sentence s in lrc)
            {
                result.Add(s.content);
                re += s.content;
            }
            return re;
            //return result;
        }
        public void getLrc(string text)
        {
            lrc = new List<Sentence>();
            string[] temp = text.Split(new char[2] { '[', ']' });
            for (int i = 1; i <= temp.Length / 2; i++)
            {
                TimeSpan t = new TimeSpan();
                bool success = TimeSpan.TryParse("00:" + temp[2 * i - 1], out t);
                if (!success) ;
                    lrc.Add(new Sentence(t, temp[2 * i].Replace("\r", "").Replace("\\n", "\n"), i));
            }
        }
        //这个可以更新currentIndex的。
        public string getCurrentSentence(double currentSeconds)
        {
            string result = "";
            int temp = 0;
            foreach (Sentence sentence in lrc)
            {
                if (currentSeconds > sentence.time.TotalSeconds)
                {
                    result = sentence.content;
                    temp = sentence.index;
                }
                else
                {
                    currentIndex = temp;
                    return result;
                }

            }
            return "";
        }

        //这个可能可以重构掉。。
        public int getLyricLines()
        {
            List<Sentence> previousPart;
            int line = 0;
            previousPart = lrc.GetRange(0, currentIndex);
            foreach (Sentence s in previousPart)
            {
                line += s.content.Split('\n').Length - 1;
            }

            return line;
        }
    }
}
