using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Sample.ProactiveBot
{
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
            var channelData = activity.GetChannelData<TeamsChannelData>();
            var tenantId = channelData.Tenant.Id;

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
            await Conversation.SendAsync(activity, () => new ProactiveDialog());
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