namespace ReinventionBot.Models
{
    public class GitUser
    {
        public GitUser(string username, string userUrl, string name, string avatarUrl)
        {
            this.UserName = username;
            this.UserUrl = userUrl;
            this.Name = name;
            this.AvatarUrl = avatarUrl;
        }

        public string UserName { get; set; }

        public string UserUrl { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }
    }
}