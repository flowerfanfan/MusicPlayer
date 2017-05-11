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
        private SQLiteConnection Conn;
        //private SQLiteConnection SongListTableConn;

        private DataBaseManager() { }

        // 获得DataBaseManager实例
        public static DataBaseManager GetDBManager()
        {
            if (DBManager == null) DBManager = new DataBaseManager();
            return DBManager;
        }

        // 创建或打开数据库
        public void LoadDatabase()
        {
            Conn = new SQLiteConnection("music_player.db");
            // 加载Songs表
            using (var stmt = Conn.Prepare(@"CREATE TABLE IF NOT EXISTS
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
            // 加载SongList表
            using (var stmt = Conn.Prepare(@"CREATE TABLE IF NOT EXISTS
                                          SongList (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                                    Name VARCHAR(200),
                                                    Number INTEGER
                                                   );"))
            {
                stmt.Step();
            }
        }

        // 返回 Songs 表中所有歌曲的信息
        public ObservableCollection<Song> GetSongs()
        {
            ObservableCollection<Song> data = new ObservableCollection<Song>();
            using (var stmt = Conn.Prepare("SELECT * FROM Songs"))
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
        public void AddSong(Song song)
        {
            using (var stmt = Conn.Prepare("INSERT INTO Songs(FilePath, Title, Artist, Album, Length, Cover) VALUES (?, ?, ?, ?, ?, ?)"))
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
                AddSong(song);
            }
        }

        // 清空 Songs 表
        public void ClearSongs()
        {
            using (var stmt = Conn.Prepare("DELETE FROM Songs"))
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

        public void AddSongList(string name)
        {
            using (var stmt = Conn.Prepare("INSERT INTO SongList(Name, Number) VALUES (?, ?)"))
            {
                stmt.Bind(1, name);
                stmt.Bind(2, 0);
                stmt.Step();
            }
        }

        public void CreateSongList(string name)
        {
            AddSongList(name);
            // 新建对应的歌单表

        }
    }
}
