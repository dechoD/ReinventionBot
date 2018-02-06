namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class GitConfirmDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var gitUsername = context.ConversationData.GetValue<string>("gitname");
            var gitAvatar= context.ConversationData.GetValue<string>("gitavatar");

            var reply = Activity.CreateMessageActivity();
            reply.ReplyToId = context.Activity.Id;
            reply.ChannelId = context.Activity.ChannelId;
            reply.Conversation = context.Activity.Conversation;

            try
            {
                reply.TextFormat = TextFormatTypes.Plain;

                reply.Attachments = new List<Attachment>
                {
                    new ThumbnailCard()
                    {
                        Title = "Is this your account?",
                        Subtitle = gitUsername,
                        Images = new List<CardImage>
                        {
                            new CardImage(gitAvatar, "Git hub avatar")
                        },
                        Buttons = new List<CardAction>
                        {
                            new CardAction() { Value = "yes", Type = ActionTypes.MessageBack, Title = "Yes" },
                            new CardAction() { Value = "no", Type = ActionTypes.MessageBack, Title = "No" }
                        }
                    }.ToAttachment()
                };

                await context.PostAsync(reply);
            }
            catch (Exception ex)
            {
                await context.PostAsync(ex.ToString());
                throw;
            }            

            context.Wait(MessageReceivedAsync);
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            bool contextResult = activity.Text == "yes" ? true : false;
            context.Done(contextResult);
        }
    }
}