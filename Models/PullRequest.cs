using Newtonsoft.Json;

namespace ReinventionBot.Models
{
    public class PullRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public User User { get; set; }
    }

    public class User
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        [JsonProperty(PropertyName = "login")]
        public string Username { get; set; }
    }
}