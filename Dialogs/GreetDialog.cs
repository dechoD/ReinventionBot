namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    [Serializable]
    public class GreetDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            // Greet the user
            await context.PostAsync($"Hello {context.Activity.From.Name}. This is the first time we meet.");
            await context.PostAsync($"My aim is to provide you some live information for the services you subscribe to.");
            await context.PostAsync($"Type subscribe to start the subscription process.");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text == "status")
            {
                await context.PostAsync("Status window navigation");
            }
            else if (activity.Text == "sub")
            {
                context.Call(new ConfirmationDialog(), this.ResumeAferConfirmationDialog);
            }
            else if (activity.Text == "unsubscribe")
            {
                await context.PostAsync("Unsubscribe window navigation");
            }
            else
            {
                await context.PostAsync("I don't understand you.");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAferConfirmationDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var resultFromNewOrder = await result;

            if (resultFromNewOrder)
            {
                await context.PostAsync("Ok now let's see what you want to receive.");
            }
            else
            {
                await context.PostAsync("Ok, no spam for you.");
            }

            // Again, wait for the next message from the user.
            context.Wait(this.MessageReceivedAsync);
        }
    }
}