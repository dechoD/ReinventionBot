﻿namespace ProactiveBot.Utilities
{
    using Microsoft.Bot.Builder.Dialogs;
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
            try
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
                        string avatarUrl = (string)json.SelectToken("avatar_url");
                        string name = (string)json.SelectToken("name");

                        return new GitUser(name, avatarUrl);
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }            
        }
    }
}