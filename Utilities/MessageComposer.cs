namespace ReinventionBot.Utilities
{
    using System;
    using System.Configuration;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    using ReinventionBot.Models;

    public static class MessageComposer
    {
        public static string SubscriptionStatus(TeamsUser user)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("# **Subscription status**\n\n");
            sb.Append("***\n\n");

            if (!user.SubscribedForRepositoryMerges && !user.SubscribedForRepositoryUpdates && !user.SubscribedForJenkinsBuildResults && !user.SubscribedForCiBuildResults)
            {
                sb.Append("You are not subscribed to any notifications.");
            }
            else
            {
                sb.Append(string.Format("* Iris repository merges by merge bot: {0}ubscribed\n\n", user.SubscribedForRepositoryMerges ? "S" : "Not s"));
                sb.Append(string.Format("* Iris repository updates by merge bot: {0}ubscribed\n\n", user.SubscribedForRepositoryUpdates ? "S" : "Not s"));
                sb.Append(string.Format("* Your Jenkins builds: {0}ubscribed\n\n", user.SubscribedForJenkinsBuildResults ? "S" : "Not s"));
                sb.Append(string.Format("* CI builds: {0}ubscribed\n\n", user.SubscribedForCiBuildResults ? "S" : "Not s"));
            }            

            return sb.ToString();
        }

        public async static Task SendMessageToUser(string userId, string conversationId, string message)
        {
            //var message = await argument;
            // Extract data from the user's message that the bot will need later to send an ad hoc message to the user. 
            // Store the extracted data in a custom class "ConversationStarter" (not shown here).

            var userAccount = new ChannelAccount(userId);
            var botAccount = new ChannelAccount("28:f46eea21-be1b-4ddf-b68d-ad8d50b7b55a", "ReinvetionBot");
            var connector = new ConnectorClient(
                new Uri(@"https://smba.trafficmanager.net/amer-client-ss.msg/"),
                ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppIdKey],
                ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppPasswordKey]
            );

            IMessageActivity returnmessage = Activity.CreateMessageActivity();
            returnmessage.ChannelId = "msteams";

            returnmessage.From = botAccount;
            returnmessage.Recipient = userAccount;
            returnmessage.Conversation = new ConversationAccount(id: conversationId);
            returnmessage.Text = message;
            returnmessage.Locale = "en-us";

            await connector.Conversations.SendToConversationAsync((Activity)returnmessage);
        }
    }
}