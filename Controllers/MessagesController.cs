namespace ReinventionBot
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.Dialogs;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using ReinventionBot.Dialogs;
    using ReinventionBot.Utilities;

    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<IHttpActionResult> Post([FromBody] Activity activity)
        {
            if (activity.GetActivityType() == ActivityTypes.Message)
            {
                await this.HandleMessage(activity);
            }
            else if (activity.Type == ActivityTypes.Event)
            {
                await this.HandleEvent(activity);
            }
            else
            {
                this.HandleSystemMessage(activity);
            }

            return this.Ok();
        }

        private async Task HandleMessage(Activity activity)
        {
            //await MessageComposer.SendMessageToUser("29:1Zg8q4E-_qtiNekcsFmWdbaih5zkCNlg4hbxMXwRFO2gbpaaLJTNDd-ifz5NYV30WV1NWvHL1lHPz-Wzg6wNt8A", "a:1FF_jJrHwxIBqzPMx4yLVjtoCVcD6o5Dkh7xqetx-rYDHSxmSmtPYMHsvC3W_qBkEVHVDOd9sewyfFYdYvGtW7kTKLQjUq7r3Cdhc5RcIqYGQbradXpl9i1mMAxPNroUD", "whatever");
            var userName = activity.From.Name;
            var userId = activity.From.Id;
            var messageText = activity.Text;

            if (userId == "MergeBot")
            {
                await MergeBotHandler.HandleMergeBotMessage(activity);                
            }
            else
            {
                await Conversation.SendAsync(activity, () => new GreetDialog());
            }
        }

        private async Task HandleEvent(Activity activity)
        {
            IEventActivity triggerEvent = activity;
            var message = JsonConvert.DeserializeObject<Message>(((JObject)triggerEvent.Value).GetValue("Message").ToString());
            var messageactivity = (Activity)message.RelatesTo.GetPostToBotMessage();

            var client = new ConnectorClient(new Uri(messageactivity.ServiceUrl));
            var triggerReply = messageactivity.CreateReply();
            triggerReply.Text = $"This is coming back from the trigger! {message.Text}";
            await client.Conversations.ReplyToActivityAsync(triggerReply);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }

    public class Message
    {
        public ConversationReference RelatesTo { get; set; }

        public String Text { get; set; }
    }
}