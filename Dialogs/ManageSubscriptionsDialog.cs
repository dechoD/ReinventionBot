namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using global::ProactiveBot.Models;
    using global::ProactiveBot.Utilities;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class ManageSubscriptionsDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("To manage a notification setting type its name - *merges*, *updates*, *ci*, *jenkins*\n\n" + 
                "To get notification setting status, type *status*. To finalize your selection enter *done*.");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);
            var messageText = activity.Text.Trim();

            if (StringHelper.Equals(messageText, "status"))
            {
                await context.PostAsync(MessageComposer.SubscriptionStatus(user));
                context.Wait(MessageReceivedAsync);
            }
            else if (StringHelper.Equals(messageText, "merges"))
            {
                user.SubscribedForRepositoryMerges = !user.SubscribedForRepositoryMerges;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("You are now {0}subscribed for merges.", !user.SubscribedForRepositoryMerges ? "un" : string.Empty));
                context.Wait(MessageReceivedAsync);
            }
            else if (StringHelper.Equals(messageText, "updates"))
            {
                user.SubscribedForRepositoryUpdates = !user.SubscribedForRepositoryUpdates;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("You are now {0}subscribed for updates.", !user.SubscribedForRepositoryUpdates ? "un" : string.Empty));
                context.Wait(MessageReceivedAsync);
            }
            else if (StringHelper.Equals(messageText, "ci"))
            {
                user.SubscribedForCiBuildResults = !user.SubscribedForCiBuildResults;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("You are now {0}subscribed for CI builds.", !user.SubscribedForCiBuildResults ? "un" : string.Empty));
                context.Wait(MessageReceivedAsync);
            }
            else if (StringHelper.Equals(messageText, "jenkins"))
            {
                if (string.IsNullOrEmpty(user.GitUsername))
                {
                    context.Call(new GitUserDialog(), ResumeAferGitUserDialog);
                }
                else
                {
                    user.SubscribedForJenkinsBuildResults = !user.SubscribedForJenkinsBuildResults;
                    await AzureTableStorage.UpdateUser(user);
                    await context.PostAsync(string.Format("You are now {0}subscribed for your Jenkins build results.", !user.SubscribedForJenkinsBuildResults ? "un" : string.Empty));
                    context.Wait(MessageReceivedAsync);
                }
            }
            else if (StringHelper.Equals(messageText, "done"))
            {
                context.Done(true);
            }
            else
            {
                await context.PostAsync("Possible commands are: status, merges, updates, ci, jenkins, done");
                context.Wait(MessageReceivedAsync);
            }
        }    

        private async Task ResumeAferGitUserDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var resultFromGitUserDialog = await result;

            if (resultFromGitUserDialog)
            {
                var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);


                user.SubscribedForJenkinsBuildResults = !user.SubscribedForJenkinsBuildResults;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("Thanks {0}.", user.GitUsername) +
                    string.Format("You are now {0}subscribed for your Jenkins build results.", !user.SubscribedForJenkinsBuildResults ? "un" : string.Empty));

                context.Wait(MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync("Without your username there is not much I can do. You can continue managing your subscriptions.");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}