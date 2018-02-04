namespace ProactiveBot.Models
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class TeamsUser : TableEntity
    {
        public TeamsUser() { }

        public TeamsUser(string name, string id)
        {
            this.PartitionKey = name;
            this.RowKey = id;

            this.Name = name;
        }

        public string Name { get; set; }

        public string GitUsername { get; set; }

        public string ConversationId { get; set; }

        public bool SubscribedForRepositoryMerges { get; set; }

        public bool SubscribedForRepositoryUpdates { get; set; }

        public bool SubscribedForJenkinsBuildResults { get; set; }

        public bool SubscribedForCiBuildResults { get; set; }

        public bool Unsubscribed { get; set; }
    }
}