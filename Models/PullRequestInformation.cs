namespace ReinventionBot.Models
{
    using Newtonsoft.Json;

    public class PullRequestInformation
    {
        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string AuthorLogin { get; set; }

        [JsonProperty(PropertyName = "updated")]
        public bool Updated { get; set; }

        [JsonProperty(PropertyName = "merged")]
        public bool Merged { get; set; }

        [JsonProperty(PropertyName = "conflicting")]
        public bool Conflicting { get; set; }
    }
}
