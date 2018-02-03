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
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var reply = activity.CreateReply("Some notifications?");
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Title = "I'm a hero card",
                    Subtitle = "Wikipedia Page",
                    Buttons = new List<CardAction>
                    {
                        new CardAction()
                        {
                            Value = @"hoi",
                            Type = "openUrl",
                            Title = "WikiPedia Page"
                        }
                    }
                }.ToAttachment()
            };

            await context.PostAsync(reply);
            context.Wait(this.MessageReceivedAsync);

            //var reply = activity.CreateReply($"Hello {activity.From.Name}. This is the first time we meet.");
            //reply.Type = ActivityTypes.Message;
            //reply.TextFormat = TextFormatTypes.Plain;

            // return our reply to the user
            //await context.PostAsync(reply);

            //context.Call(new ConfirmationDialog(), this.ResumeAferConfirmationDialog);
        }

        private async Task ResumeAferConfirmationDialog(IDialogContext context, IAwaitable<bool> result)
        {
            // Store the value that NewOrderDialog returned. 
            // (At this point, new order dialog has finished and returned some value to use within the root dialog.)
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