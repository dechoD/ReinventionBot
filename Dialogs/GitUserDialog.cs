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
    public class GitUserDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("I will need your GitHub username. Please type it, I will let you confirm it after that.");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text == "done" || activity.Text == "Done")
            {
                context.Done(false);
            }
            else
            { 
                GitUser GitHubUserInfo = await HttpHelper.GetGitUserInformation(activity.Text);            

                if (GitHubUserInfo == null)
                {
                    await context.PostAsync("I was unable to find your github profile. Can you check it and type it again please.");
                    await context.PostAsync("If you want to abort type \"done\".");
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    context.ConversationData.SetValue("gitname", GitHubUserInfo.Name);
                    context.ConversationData.SetValue("gitavatar", GitHubUserInfo.AvatarUrl);

                    context.Call(new GitConfirmDialog(), ResumeAferConfirmDialog);
                }
            }
        }

        private async Task ResumeAferConfirmDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var resultFromGitHubConfirmation = await result;

            if (resultFromGitHubConfirmation)
            {
                var tableUser = await AzureTableStorage.GetUserById(context.Activity.From.Id);
                tableUser.GitUsername = context.ConversationData.GetValue<string>("gitname");

                await AzureTableStorage.UpdateUser(tableUser);

                context.Done(true);
            }
            else
            {
                await context.PostAsync("You can try with a different username or just type \"done\".");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}