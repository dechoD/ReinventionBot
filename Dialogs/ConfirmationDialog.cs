namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using global::ProactiveBot.Models;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;

    [Serializable]
    public class ConfirmationDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Do you want me to remember you so I can send you notifications?");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text == "y")
            {
                try
                {
                    // Retrieve storage account from connection string.
                    var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]); // If you're running this bot locally, make sure you have this appSetting in your web.config

                    // Create the queue client.
                    var tableClient = storageAccount.CreateCloudTableClient();

                    // Retrieve a reference to a table.
                    var table = tableClient.GetTableReference("UsersTable");

                    // Create the table if it doesn't already exist.
                    await table.CreateIfNotExistsAsync();

                    // Create a user and add it to the table.
                    TeamsUser user = new TeamsUser(message.From.Name, message.From.Id);

                    TableOperation insertUser = TableOperation.Insert(user);
                    table.Execute(insertUser);
                }
                catch (Exception ex)
                {
                    await context.PostAsync(ex.ToString());

                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        await context.PostAsync(ex.ToString());
                    }
                }
                

                context.Done(true);
            }
            else if (message.Text == "No")
            {
                context.Done(false);
            }
            else
            {
                await context.PostAsync("It's either Yes on No. I do not understand anything else.");
                await context.PostAsync("Sooo, you want to receive notifications?");

                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}