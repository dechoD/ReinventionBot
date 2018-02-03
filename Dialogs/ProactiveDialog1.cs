using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.ConnectorEx;
using System.Net.Http;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;

namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    [Serializable]
    public class ProactiveDialog1 : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            // Extract data from the user's message that the bot will need later to send an ad hoc message to the user. 
            // Store the extracted data in a custom class "ConversationStarter" (not shown here).

            var userAccount = new ChannelAccount("29:1Zg8q4E-_qtiNekcsFmWdbaih5zkCNlg4hbxMXwRFO2gbpaaLJTNDd-ifz5NYV30WV1NWvHL1lHPz-Wzg6wNt8A", "Decho Dechev");
            var botAccount = new ChannelAccount("28:f46eea21-be1b-4ddf-b68d-ad8d50b7b55a", "ReinvetionBot");
            var connector = new ConnectorClient(
                new Uri(@"https://smba.trafficmanager.net/amer-client-ss.msg/"),
                ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppIdKey],
                ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppPasswordKey]
            );

            IMessageActivity returnmessage = Activity.CreateMessageActivity();
            returnmessage.ChannelId = "msteams";

            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: "a:1FF_jJrHwxIBqzPMx4yLVjtoCVcD6o5Dkh7xqetx-rYDHSxmSmtPYMHsvC3W_qBkEVHVDOd9sewyfFYdYvGtW7kTKLQjUq7r3Cdhc5RcIqYGQbradXpl9i1mMAxPNroUD");
            message.Text = "Hello, this is a notification. With something else";
            message.Locale = "en-us";
            await connector.Conversations.SendToConversationAsync((Activity)message);

            // Create a queue Message
            var queueMessage = new Message
            {
                RelatesTo = context.Activity.ToConversationReference(),
                Text = message.Text
            };

            // write the queue Message to the queue
            await AddMessageToQueueAsync(JsonConvert.SerializeObject(queueMessage), context);

            await context.PostAsync(JsonConvert.SerializeObject(context.Activity));

            //if (context.Activity.From.Id == "MergeBot")
            //{
            //var channelData = new TeamsChannelData { Channel = new ChannelInfo("19:be90b3fe9acb46079c9f06595fb5ae6d@thread.skype") };
            //IMessageActivity newMessage = Activity.CreateMessageActivity();
            //newMessage.Type = ActivityTypes.Message;
            //newMessage.Text = message.Text;
            //ConversationParameters conversationParams = new ConversationParameters(
            //    isGroup: true,
            //    bot: null,
            //    members: null,
            //    topicName: "Merge run status report",
            //    activity: (Activity)newMessage,
            //    channelData: channelData
            //);

            //var connector = new ConnectorClient(
            //    new Uri(@"https://smba.trafficmanager.net/amer-client-ss.msg/"),
            //    ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppIdKey],
            //    ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppPasswordKey]
            //);
            //var result = await connector.Conversations.CreateConversationAsync(conversationParams);
            //}
            //else {
            //    // Create a queue Message
            //    var queueMessage = new Message
            //    {
            //        RelatesTo = context.Activity.ToConversationReference(),
            //        Text = message.Text
            //    };

            //    // write the queue Message to the queue
            //    await AddMessageToQueueAsync(JsonConvert.SerializeObject(queueMessage), context);

            //    await context.PostAsync(JsonConvert.SerializeObject(context.Activity));
            //}

            context.Wait(MessageReceivedAsync);
        }

        public static async Task AddMessageToQueueAsync(string message, IDialogContext context)
        {
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureWebJobsStorage"]); // If you're running this bot locally, make sure you have this appSetting in your web.config

            // Create the queue client.
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            var queue = queueClient.GetQueueReference("bot-queue");

            // Create the queue if it doesn't already exist.
            await queue.CreateIfNotExistsAsync();

            // Create a message and add it to the queue.
            var queuemessage = new CloudQueueMessage(message);
            await queue.AddMessageAsync(queuemessage);

            //try
            //{
            //    var queueMessages = await queue.GetMessagesAsync(100);
            //    var messages = queueMessages.Select(m => m.AsString);

            //    await context.PostAsync(string.Join("\n", messages));
            //}
            //catch (Exception e)
            //{
            //    var exception = e;

            //    while (exception.InnerException != null)
            //    {
            //        exception = exception.InnerException;
            //    }

            //    await context.PostAsync(exception.Message);
            //}
        }

    }
}