namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Serializable]
    public class ConfirmationDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Do you want me to remember you so I can send you notifications?");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text == "Yes")
            {
                context.Done(true);
            }
            else if (message.Text == "No")
            {
                context.Done(false);
            }
            else
            {
                await context.PostAsync("It's either Yes on No. I do not understand anything else.");
                await context.PostAsync("Sooo, you want to receive notifications?");

                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}