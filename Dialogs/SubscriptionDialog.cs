namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using global::ProactiveBot.Models;
    using global::ProactiveBot.Utilities;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;

    [Serializable]
    public class SubscriptionDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);

            await context.PostAsync($"Ok, {user.Name}. Let's manage your notification subscriptions.");
            await context.PostAsync("If you want to manage notifications type manage.");
            await context.PostAsync("If you want to stop receiving all notifications type unsubscribe.");
            await context.PostAsync("You can always type done if you don't want to change anything.");

            context.Wait(this.MessageReceivedAsync);            
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result as Activity;

                var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);

                if (message.Text == "manage")
                {
                    if (user.Unsubscribed)
                    {
                        user.Unsubscribed = false;
                        await AzureTableStorage.UpdateUser(user);
                        await context.PostAsync("You were unsubscribed, so before managing the notifications your subscription was renewed.");
                    }                    

                    context.Call(new ManageSubscriptionsDialog(), this.ResumeAfterManageSubscriptionsDialog);
                }
                else if (message.Text == "unsubscribe")
                {
                    user.Unsubscribed = true;
                    await AzureTableStorage.UpdateUser(user);

                    context.Done(false);
                }
                else if (message.Text == "done")
                {
                    context.Done(true);
                }
                else
                {
                    await context.PostAsync("It's either subscribe, unsubscribe or done. I do not understand anything else.");
                    await context.PostAsync("Sooo, you want to receive notifications?");

                    context.Wait(this.MessageReceivedAsync);
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync(ex.ToString());
                throw;
            }            
        }

        private async Task ResumeAfterManageSubscriptionsDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var resultFromManageSubscriptionsDialog = await result;
            context.Done(true);
        }
    }
}