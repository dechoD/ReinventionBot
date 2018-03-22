using Newtonsoft.Json;
using System.Collections.Generic;

namespace ReinventionBot.Models
{
    public class PullRequest
    {
        public string Title { get; set; }

        [JsonProperty(PropertyName = "body")]
        public string Description { get; set; }

        public User User { get; set; }

        public IEnumerable<Review> Reviews { get; set; }

        public IEnumerable<User> RequestedReviewers { get; set; }

        [JsonProperty(PropertyName = "html_url")]
        public string HtmlUrl { get; set; }
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