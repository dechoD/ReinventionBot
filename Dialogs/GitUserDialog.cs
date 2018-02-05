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
            await context.PostAsync("I will need your jenkins username. Please type it, I will let you confirm it after that.");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var response = await HttpHelper.GetGitUserInformation(activity.Text);

            context.ConversationData.SetValue("gitname", response.Name);
            context.ConversationData.SetValue("gitavatar", response.AvatarUrl);

            context.Call(new GitConfirmDialog(), ResumeAferConfirmDialog);
        }

        private async Task ResumeAferConfirmDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var resultFromSubscriptionsDialog = await result;

            if (resultFromSubscriptionsDialog)
            {
                var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);
                user.GitUsername = context.ConversationData.GetValue<string>("gitname");

                await AzureTableStorage.UpdateUser(user);

                context.Done(true);
            }
            else
            {
                context.Done(false);
            }
        }
    }
}