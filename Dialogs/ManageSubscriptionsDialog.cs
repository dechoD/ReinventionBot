namespace ReinventionBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using ReinventionBot.Utilities;

    [Serializable]
    public class ManageSubscriptionsDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("To manage a notification setting type its name - *merges*, *updates*, *reviews*, *ci*, *jenkins*\n\n" + 
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
            else if (StringHelper.Equals(messageText, "reviews"))
            {
                user.SubscribedForRequestedReviews = !user.SubscribedForRequestedReviews;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("You are now {0}subscribed for review requests.", !user.SubscribedForRequestedReviews ? "un" : string.Empty));
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
                await context.PostAsync("Possible commands are: status, merges, reviews, updates, ci, jenkins, done");
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
                await context.PostAsync(string.Format("Thanks {0}.\n\n", user.GitUsername) +
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