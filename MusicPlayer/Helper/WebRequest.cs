using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace MusicPlayer.Helper
{
    static class WebRequest
    {
        public static string  getLyric(string name)
        {
            List<string> Ids = GetSongId(name);
            var id = Ids[0];
            Uri addr = new Uri("http://music.163.com/api/song/lyric?os=pc&id=" + id + "&lv=-1&kv=-1&tv=-1");
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            var response = client.GetStringAsync(addr).Result;
            JObject jresult = (JObject)JsonConvert.DeserializeObject(response);
            if (jresult.Count == 5)
                return "No lyric";
            else
                return jresult["lrc"].ToString();
        }
        public static List<string> GetSongId(string name)
        {

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

            paramList.Add(new KeyValuePair<string, string>("s", name));
            paramList.Add(new KeyValuePair<string, string>("offset", "0"));
            paramList.Add(new KeyValuePair<string, string>("limit", "3"));
            paramList.Add(new KeyValuePair<string, string>("type", "1"));

            List<string> Ids = new List<string>();
            var response = client.PostAsync(new Uri("http://music.163.com/api/search/pc"), new FormUrlEncodedContent(paramList)).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            JObject jresult = (JObject)JsonConvert.DeserializeObject(result);
            JArray songs = (JArray)jresult["result"]["songs"];
            foreach (var s in songs)
            {
                Ids.Add(s["id"].ToString());
            }
            return Ids;
        }
    }
}
