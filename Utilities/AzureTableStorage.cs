namespace ProactiveBot.Utilities
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using ProactiveBot.Models;
    using System.Configuration;
    using System.Threading.Tasks;

    public static class AzureTableStorage
    {
        public static async Task InsertUser(string name, string id, string conversationId)
        {
            // If you're running this bot locally, make sure you have this appSetting in your web.config
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]); 

            // Create or retrieve the table reference
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("UsersTable");
            await table.CreateIfNotExistsAsync();

            // Create a user and add it to the table.
            TeamsUser user = new TeamsUser(name, id);
            user.ConversationId = conversationId;

            TableOperation insertUser = TableOperation.Insert(user);
            table.Execute(insertUser);
        }

        public static async Task<bool> UserExists(string id)
        {
            bool userExists = false;
            // If you're running this bot locally, make sure you have this appSetting in your web.config
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]);

            // Create or retrieve the table reference
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("UsersTable");
            await table.CreateIfNotExistsAsync();

            TableQuery<TeamsUser> query = new TableQuery<TeamsUser>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));
            var result = table.ExecuteQuery(query);

            int count = 0;
            foreach (var item in result)
            {
                count++;
            }

            if (count == 1)
            {
                userExists = true;
            }

            return userExists;
        }

        public static async Task<TeamsUser> GetUserById(string id)
        {
            TeamsUser user = null;

            // If you're running this bot locally, make sure you have this appSetting in your web.config
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]);

            // Create or retrieve the table reference
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("UsersTable");
            await table.CreateIfNotExistsAsync();

            TableQuery<TeamsUser> query = new TableQuery<TeamsUser>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));
            var result = table.ExecuteQuery(query);

            var usersFound = 0;
            if (result != null)
            {
                foreach (var entry in result)
                {
                    usersFound++;
                    user = entry;
                }
            }

            if (usersFound > 1)
            {
                throw new StorageException("More than one user found. This should not happen.");
            }

            return user;
        }

        public static async Task UpdateUser(TeamsUser user)
        {
            // If you're running this bot locally, make sure you have this appSetting in your web.config
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]);

            // Create or retrieve the table reference
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("UsersTable");

            if (user != null)
            {
                TableOperation update = TableOperation.Replace(user);
                table.Execute(update);
            }
        }
    }
}