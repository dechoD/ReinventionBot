﻿namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using global::ProactiveBot.Utilities;
    using Microsoft.Bot.Builder.Dialogs;

    [Serializable]
    public class StatusDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);

            if (user.Unsubscribed)
            {
                await context.PostAsync($"You are unsubscribed from all messages.");
                context.Done(true);
            }
            else
            {
                await context.PostAsync(MessageComposer.SubscriptionStatus(user));
                context.Done(true);
            }            
        }
    }
}