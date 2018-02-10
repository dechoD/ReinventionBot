namespace ProactiveBot.Utilities
{
    using Newtonsoft.Json.Linq;
    using ProactiveBot.Models;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpHelper
    {
        public static async Task<GitUser> GetGitUserInformation(string username)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = string.Format("https://api.github.com/users/{0}", username);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.UserAgent = "Mozilla/5.0";

                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var res = await reader.ReadToEndAsync();

                    JToken json = JObject.Parse(res);
                    string login = (string)json.SelectToken("login");
                    string userUrl = (string)json.SelectToken("html_url");
                    string avatarUrl = (string)json.SelectToken("avatar_url");

                    string name = string.Empty;
                    try
                    {
                        name = (string)json.SelectToken("name");
                    }
                    catch (System.Exception)
                    {
                    }

                    return new GitUser(login, userUrl, name, avatarUrl);
                }
            }  
        }
    }
}