using MusicPlayer.Models;
using SQLitePCL;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;

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

        private DataBaseManager()
        {
            Conn = new SQLiteConnection("music_player.db");
        }

        // 获得DataBaseManager实例
        public static DataBaseManager GetDBManager()
        {
            if (DBManager == null) DBManager = new DataBaseManager();
            return DBManager;
        }

        // 创建或打开数据库
        public void LoadDatabase()
        {
            // 加载_Songs_表（初始表）
            using (var stmt = Conn.Prepare(@"CREATE TABLE IF NOT EXISTS
                                             `_Songs_` (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
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
            // 加载SongLists表
            using (var stmt = Conn.Prepare(@"CREATE TABLE IF NOT EXISTS
                                             SongLists (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                                        Name VARCHAR(200),
                                                        Number INTEGER
                                                       );"))
            {
                stmt.Step();
            }
            // 加载_FavoriteSongs_表
            using (var stmt = Conn.Prepare(@"CREATE TABLE IF NOT EXISTS
                                             `_FavoriteSongs_` (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
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

        // 返回歌曲表中所有歌曲的数据
        public ObservableCollection<Song> GetSongs(string listTable)
        {
            ObservableCollection<Song> data = new ObservableCollection<Song>();
            using (var stmt = Conn.Prepare("SELECT * FROM `" + listTable + "`"))
            {
                while (stmt.Step() == SQLiteResult.ROW)
                {
                    data.Add(new Song((string)stmt["FilePath"], (string)stmt["Title"],
                        (string)stmt["Artist"], (string)stmt["Album"], (string)stmt["Length"], (byte[])stmt["Cover"]));
                }
            }
            return data;
        }

        // 返回 SongList 表中所有歌单及歌单对应的表中所有歌曲的数据
        public void GetSongLists(ObservableCollection<SongList> songLists, Dictionary<string, ObservableCollection<Song>> songsInList)
        {
            using (var stmt = Conn.Prepare("SELECT * FROM SongLists"))
            {
                while (stmt.Step() == SQLiteResult.ROW)
                {
                    songLists.Add(new SongList((string)stmt["Name"], Convert.ToInt32((Int64)stmt["Number"])));
                }
            }
            foreach (SongList songList in songLists)
            {
                string name = songList.Name;
                if (!songsInList.ContainsKey(name))
                {
                    songsInList.Add(name, GetSongs(name));
                }
            }
        }

        // 添加一首歌曲的信息
        // 插入某首歌到某个歌单中
        public void AddSong(Song song, string listName)
        {
            using (var stmt = Conn.Prepare(@"INSERT
                                             INTO `" + listName +
                                            "`(FilePath, Title, Artist, Album, Length, Cover)" +
                                            "VALUES (?, ?, ?, ?, ?, ?)"))
            {
                stmt.Bind(1, song.FilePath);
                stmt.Bind(2, song.Title);
                stmt.Bind(3, song.Artist);
                stmt.Bind(4, song.Album);
                stmt.Bind(5, song.Length);
                stmt.Bind(6, song.CoverBytes);
                stmt.Step();
            }
            if (listName != "_Songs_" && listName != "_FavoriteSongs_")
            {
                using (var stmt = Conn.Prepare(@"UPDATE SongLists SET Number = ? WHERE Name = ?"))
                {
                    stmt.Bind(1, GetNumberInList(listName) + 1);
                    stmt.Bind(2, listName);
                    stmt.Step();
                }
            }
        }

        private int GetNumberInList(string listName)
        {
            int number;
            using (var stmt = Conn.Prepare(@"SELECT Number FROM SongLists WHERE Name = ?"))
            {
                stmt.Bind(1, listName);
                stmt.Step();
                number = Convert.ToInt32((Int64)stmt["Number"]);
            }
            return number;
        }

        // 添加一批歌曲的信息
        public void AddSongs(ObservableCollection<Song> songs, string listName)
        {
            foreach (Song song in songs)
            {
                AddSong(song, listName);
            }
            if (listName != "_Songs_")
            {
                int number;
                using (var stmt = Conn.Prepare(@"SELECT Number FROM SongLists WHERE Name = ?"))
                {
                    stmt.Bind(1, listName);
                    stmt.Step();
                    number = Convert.ToInt32((Int64)stmt["Number"]);
                }
                using (var stmt = Conn.Prepare(@"UPDATE SongLists SET Number = ? WHERE Name = ?"))
                {
                    stmt.Bind(1, number + songs.Count);
                    stmt.Bind(2, listName);
                    stmt.Step();
                }
            }
        }

        // 在SongLists表中添加一个新歌单
        public void AddSongList(string listName)
        {
            using (var stmt = Conn.Prepare(@"INSERT INTO SongLists (Name, Number) VALUES (?, ?)"))
            {
                stmt.Bind(1, listName);
                stmt.Bind(2, 0);
                stmt.Step();
            }
        }

        // 新建一个的歌单表
        public void CreateSongList(string listName)
        {
            AddSongList(listName);
            using (var stmt = Conn.Prepare("CREATE TABLE IF NOT EXISTS `" +
                                            listName + "`" + @" (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
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

        // 清空歌曲表
        public void ClearSongs(string listName)
        {
            using (var stmt = Conn.Prepare("DELETE FROM `" + listName + "`"))
            {
                stmt.Step();
            }
        }

        // 删除一个歌单表
        public void DeleteSongList(string listName)
        {
            using (var stmt = Conn.Prepare("DELETE FROM SongLists WHERE Name = ?"))
            {
                stmt.Bind(1, listName);
                stmt.Step();
            }
            using (var stmt = Conn.Prepare("DROP TABLE `" + listName +"`"))
            {
                stmt.Step();
            }
        }

        // 删除歌曲表中的一首歌
        public void DeleteSong(string listName, Song song)
        {
            using (var stmt = Conn.Prepare("DELETE FROM `" + listName + "` WHERE FilePath = ?"))
            {
                stmt.Bind(1, song.FilePath);
                stmt.Step();
            }
            if (listName != "_Songs_" && listName != "_FavoriteSongs_")
            {
                using (var stmt = Conn.Prepare("UPDATE SongLists SET Number = ? WHERE Name = ?"))
                {
                    stmt.Bind(1, GetNumberInList(listName) - 1);
                    stmt.Bind(2, listName);
                    stmt.Step();
                }
            }
        }
    }
}