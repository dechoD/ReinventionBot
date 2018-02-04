namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using global::ProactiveBot.Models;
    using global::ProactiveBot.Utilities;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class StatusDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);

            await context.PostAsync("Your current subscription status is as follows:");
            if (user.Unsubscribed)
            {
                await context.PostAsync($"You are unsubscribed from all messages.");
                context.Done(true);
            }
            else
            {
                await context.PostAsync(this.GetSubscriptionsMessage(user));
                context.Done(true);
            }            
        }

        private string GetSubscriptionsMessage(TeamsUser user)
        {
            StringBuilder sb = new StringBuilder();

            if (user.SubscribedForRepositoryUpdates)
            {
                sb.AppendLine("Subscribed for Iris repository updates by merge bot.");
            }

            if (user.SubscribedForRepositoryMerges)
            {
                sb.AppendLine("Subscribed for Iris repository merges by merge bot.");
            }

            if (user.SubscribedForJenkinsBuildResults)
            {
                sb.AppendLine("Subscribed for Jenkins build results.");
            }

            if (user.SubscribedForCiBuildResults)
            {
                sb.AppendLine("Subscribed for CI build results.");
            }

            if (string.IsNullOrEmpty(sb.ToString()))
            {
                sb.AppendLine("You are not subscribed to any notifications.");
            }

            return sb.ToString();
        }
    }
}