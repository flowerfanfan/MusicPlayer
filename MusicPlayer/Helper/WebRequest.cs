using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace MusicPlayer.Helper
{
    static class WebRequest
    {
        static void GetSongId(string name)
        {

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

            paramList.Add(new KeyValuePair<string, string>("s", name));
            paramList.Add(new KeyValuePair<string, string>("offset", "0"));
            paramList.Add(new KeyValuePair<string, string>("limit", "1"));
            paramList.Add(new KeyValuePair<string, string>("type", "1"));


            var response = client.PostAsync(new Uri("http://music.163.com/api/search/pc"), new FormUrlEncodedContent(paramList)).Result;
            var result = response.Content.ReadAsStringAsync().Result;

        }
    }
}
