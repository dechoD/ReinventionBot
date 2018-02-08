namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using global::ProactiveBot.Utilities;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    public class GreetDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var userName = context.Activity.From.Name;
            var userId = context.Activity.From.Id;
            var userExists = await AzureTableStorage.UserExists(userId);

            // Greet the user
            if (userExists)
            {
                await context.PostAsync($"Hello again {userName}. Nice to see you again.");
            }
            else
            {
                await context.PostAsync($"Hello {context.Activity.From.Name}. This is the first time we meet.\n\n" +
                    $"My aim is to provide you some live information for the services you subscribe to.");

                await AzureTableStorage.InsertUser(context.Activity.From.Name, context.Activity.From.Id, context.Activity.Conversation.Id);
            }            

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var messageText = activity.Text;

            if (StringHelper.Equals(messageText, "status"))
            {
                context.Call(new StatusDialog(), ResumeAferStatusDialog);
            }
            else if (StringHelper.Equals(messageText, "subscribe"))
            {
                context.Call(new SubscriptionDialog(), ResumeAferSubscriptionDialog);
            }
            else
            {
                await context.PostAsync("Ok. I don't understand you.\n\n" +
                    "Currently you can type *status* or *subscribe* to check or manage your subscriptions");

                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAferSubscriptionDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var resultFromSubscriptionsDialog = await result;

            if (resultFromSubscriptionsDialog)
            {
                await context.PostAsync("Ok now you will receive the notifications you subscribed to. Thanks.");
            }
            else
            {
                await context.PostAsync("Ok, no spam for you.\n\n" +
                    "You are unsubscribed from all notifications.");
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAferStatusDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var resultFromStatus = await result;

            context.Wait(MessageReceivedAsync);
        }
    }
}