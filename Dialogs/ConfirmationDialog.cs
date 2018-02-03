namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Serializable]
    public class ConfirmationDialog : IDialog<string>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text == "Yes")
            {
                context.Done("Yes");
            }
            else if (activity.Text == "No")
            {
                context.Done("No");
            }

            // return our reply to the user
            //await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }
    }
}