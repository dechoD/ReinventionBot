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
            await context.PostAsync("To manage a notification setting type it's name. merges, updates, ci, jenkins");
            await context.PostAsync("To get notification setting status, type status. To finalize your selection enter done.");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);

            if (activity.Text == "status")
            {
                var reply = activity.CreateReply("Current Notification Status.");

                reply = GetSubscriptionsReply(user, reply);
                await context.PostAsync(reply);
                context.Wait(MessageReceivedAsync);
            }
            else if (activity.Text == "merges")
            {
                user.SubscribedForRepositoryMerges = !user.SubscribedForRepositoryMerges;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("You are now {0}subscribed for merges.", !user.SubscribedForRepositoryMerges ? "un" : string.Empty));
                context.Wait(MessageReceivedAsync);
            }
            else if (activity.Text == "updates")
            {
                user.SubscribedForRepositoryUpdates = !user.SubscribedForRepositoryUpdates;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("You are now {0}subscribed for updates.", !user.SubscribedForRepositoryUpdates ? "un" : string.Empty));
                context.Wait(MessageReceivedAsync);
            }
            else if (activity.Text == "ci")
            {
                user.SubscribedForCiBuildResults = !user.SubscribedForCiBuildResults;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("You are now {0}subscribed for CI builds.", !user.SubscribedForCiBuildResults ? "un" : string.Empty));
                context.Wait(MessageReceivedAsync);
            }
            else if (activity.Text == "jenkins")
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
            else if (activity.Text == "done")
            {
                context.Done(true);
            }
            else
            {
                await context.PostAsync("I don't understand. Possible commands are: status, merges, updates, ci, jenkins, done");
                context.Wait(MessageReceivedAsync);
            }
        }        

        private Activity GetSubscriptionsReply(TeamsUser user, Activity reply)
        {
            reply.Type = ActivityTypes.Message;
            reply.TextFormat = TextFormatTypes.Plain;

            var mergeValue = string.Format("Iris repository merges by merge bot: {0}ubscribed", user.SubscribedForRepositoryMerges ? "S" : "Not s");
            var updateValue = string.Format("Iris repository updates by merge bot: {0}ubscribed", user.SubscribedForRepositoryUpdates ? "S" : "Not s");
            var jenkinsValue = string.Format("Your Jenkins builds: {0}ubscribed", user.SubscribedForJenkinsBuildResults ? "S" : "Not s");
            var ciValue = string.Format("CI builds: {0}ubscribed", user.SubscribedForCiBuildResults ? "S" : "Not s");

            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Title = "Want to receive notifications?",
                    Subtitle = "We will configure the verbosity later.",
                    Buttons = new List<CardAction>
                    {
                        new CardAction() { Value = "merges", Type = ActionTypes.ImBack, Title = mergeValue },
                        new CardAction() { Value = "updates", Type = ActionTypes.ImBack, Title = updateValue },
                        new CardAction() { Value = "jenkins", Type = ActionTypes.ImBack, Title = jenkinsValue },
                        new CardAction() { Value = "ci", Type = ActionTypes.ImBack, Title = ciValue }
                    }
                }.ToAttachment()
            };

            return reply;
        }

        private async Task ResumeAferGitUserDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var resultFromGitUserDialog = await result;

            if (resultFromGitUserDialog)
            {
                var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);

                await context.PostAsync(string.Format("Thanks {0}.", user.GitUsername));

                user.SubscribedForJenkinsBuildResults = !user.SubscribedForJenkinsBuildResults;
                await AzureTableStorage.UpdateUser(user);
                await context.PostAsync(string.Format("You are now {0}subscribed for your Jenkins build results.", !user.SubscribedForJenkinsBuildResults ? "un" : string.Empty));
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