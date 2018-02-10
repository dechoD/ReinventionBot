namespace ReinventionBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    using ReinventionBot.Utilities;

    [Serializable]
    public class SubscriptionDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);

            await context.PostAsync($"Ok, {user.Name}. Let's manage your notification subscriptions.\n\n" +
                "If you want to manage notifications type *manage*.\n\n" +
                "If you want to stop receiving all notifications type *unsubscribe*.\n\n" +
                "You can always type *done* if you don't want to change anything.");

            context.Wait(this.MessageReceivedAsync);            
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                var messageText = activity.Text;

                var user = await AzureTableStorage.GetUserById(context.Activity.From.Id);

                if (StringHelper.Equals(messageText, "manage"))
                {
                    if (user.Unsubscribed)
                    {
                        user.Unsubscribed = false;
                        await AzureTableStorage.UpdateUser(user);
                        await context.PostAsync("You were unsubscribed, so before managing the notifications your subscription was renewed.");
                    }                    

                    context.Call(new ManageSubscriptionsDialog(), this.ResumeAfterManageSubscriptionsDialog);
                }
                else if (StringHelper.Equals(messageText, "unsubscribe"))
                {
                    user.Unsubscribed = true;
                    await AzureTableStorage.UpdateUser(user);

                    context.Done(false);
                }
                else if (StringHelper.Equals(messageText, "done"))
                {
                    context.Done(true);
                }
                else
                {
                    await context.PostAsync("It's either manage or unsubscribe. I do not understand anything else." +
                        "Sooo, you want to receive notifications?");

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