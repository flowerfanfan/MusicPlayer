using MusicPlayer.Models;
using SQLitePCL;
using System.Collections.ObjectModel;

namespace MusicPlayer.DataBase
{
    public class DataBaseManager
    {
        // 单例模式
        // 在需要使用数据库的地方加上
        // DataBaseManager DBManager = DataBaseManager.GetDBManager();
        // 即可通过 DBManager 来进行数据库操作
        private static DataBaseManager DBManager;

        private SQLiteConnection conn { get; set; }

        private DataBaseManager() { }

        // 获得DataBaseManager实例
        public static DataBaseManager GetDBManager()
        {
            if (DBManager == null) DBManager = new DataBaseManager();
            return DBManager;
        }

        // 创建或打开数据库
        // 数据库内有一个 Songs 表
        public void LoadDatabase()
        {
            conn = new SQLiteConnection("songs.db");
            using (var stmt = conn.Prepare(@"CREATE TABLE IF NOT EXISTS
                                             Songs (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                                    FilePath VARCHAR(300),
                                                    Title VARCHAR(200),
                                                    Artist VARCHAR(200),
                                                    Album VARCHAR(200),
                                                    Length VARCHAR(10),
                                                    Cover BLOB
                                                   );"))
            {
                stmt.Step();
            }
        }

        // 返回 Songs 表中所有歌曲的信息
        public ObservableCollection<Song> GetData()
        {
            ObservableCollection<Song> data = new ObservableCollection<Song>();
            using (var stmt = conn.Prepare("SELECT * FROM Songs"))
            {
                while (stmt.Step() == SQLiteResult.ROW)
                {
                    data.Add(new Song((string)stmt["FilePath"], (string)stmt["Title"],
                        (string)stmt["Artist"], (string)stmt["Album"], (string)stmt["Length"], (byte[])stmt["Cover"]));
                }
            }
            return data;
        }

        // 添加一首歌曲的信息
        public void Add(Song song)
        {
            using (var stmt = conn.Prepare("INSERT INTO Songs(FilePath, Title, Artist, Album, Length, Cover) VALUES (?, ?, ?, ?, ?, ?)"))
            {
                stmt.Bind(1, song.FilePath);
                stmt.Bind(2, song.Title);
                stmt.Bind(3, song.Artist);
                stmt.Bind(4, song.Album);
                stmt.Bind(5, song.Length);
                stmt.Bind(6, song.CoverBytes);
                stmt.Step();
            }
        }

        // 添加一批歌曲的信息
        public void AddSongs(ObservableCollection<Song> songs)
        {
            foreach (Song song in songs)
            {
                Add(song);
            }
        }

        // 清空 Songs 表
        public void ClearData()
        {
            using (var stmt = conn.Prepare("DELETE FROM Songs"))
            {
                stmt.Step();
            }
        }

        //// 删除一首歌曲的信息
        //public void Delete(string filePath)
        //{
        //    using (var stmt = conn.Prepare("DELETE FROM Songs WHERE FilePath = ?"))
        //    {
        //        stmt.Bind(1, filePath);
        //        stmt.Step();
        //    }
        //}
    }
}
