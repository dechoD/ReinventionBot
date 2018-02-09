namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::ProactiveBot.Utilities;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class GitConfirmDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var gitUserName = context.ConversationData.GetValue<string>("gituser");
            var gitName = context.ConversationData.GetValue<string>("gitname");
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
                        Subtitle = gitUserName,
                        Text = gitName,
                        Images = new List<CardImage>
                        {
                            new CardImage(gitAvatar, "Git hub avatar")
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
            var messageText = activity.Text;

            if (StringHelper.Equals(messageText, "yes"))
            {
                context.Done(true);
            }
            else if (StringHelper.Equals(messageText, "no"))
            {
                context.Done(false);
            }
            else
            {
                await context.PostAsync("*Yes* or *no* please.\n\n" +
                    "Is this your GitHub user?");

                context.Wait(MessageReceivedAsync);
            }
        }
    }
}