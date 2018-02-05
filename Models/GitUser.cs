namespace ProactiveBot.Models
{
    public class GitUser
    {
        public GitUser(string name, string url)
        {
            this.Name = name;
            this.AvatarUrl = url;
        }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }
    }
}