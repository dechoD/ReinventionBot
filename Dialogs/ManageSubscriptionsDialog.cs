namespace Microsoft.Bot.Sample.ProactiveBot.Dialogs
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using global::ProactiveBot.Models;
    using global::ProactiveBot.Utilities;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class ManageSubscriptionsDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("in manage subscriptions");
            context.Done(true);
        }        
    }
}