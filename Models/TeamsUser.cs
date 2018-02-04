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
        }
    }
}