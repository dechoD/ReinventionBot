namespace ProactiveBot.Models
{
    public class GitUser
    {
        public GitUser(string username, string name, string url)
        {
            this.UserName = username;
            this.Name = name;
            this.AvatarUrl = url;
        }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }
    }
}