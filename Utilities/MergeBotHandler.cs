namespace Microsoft.Bot.ReinventionBot.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    using Models;
    using Newtonsoft.Json;
    using Microsoft.Bot.ReinventionBot.Extensions;

    public static class MergeBotHandler
    {
        public static async Task HandleMergeBotMessage(Activity activity)
        {
            try
            {
                var pullRequestsProperty = activity.Properties["data"];

                if (pullRequestsProperty.HasValues)
                {
                    var pullRequestDatas = JsonConvert.DeserializeObject<PullRequestInformation[]>(pullRequestsProperty.ToString());

                    // getting the users here in order not to pull them from the db on every PR
                    var userSubscribedForRequestedReviews = await AzureTableStorage.GetUsersBySubscription(TableStorageSubscriptionColumns.SubscribedForRequestedReviews);

                    foreach (var pullRequestData in pullRequestDatas)
                    {
                        foreach (var pullRequest in pullRequestData.PullRequests)
                        {
                            if (pullRequestData.Status == PullRequestStatus.Merged)
                            {
                                var pattern = @"\s+#(\d{6})";
                                var ids = new HashSet<string>();

                                ids.AddRange(GetMatchedItemsIds(pullRequest.Description, pattern));
                                ids.AddRange(GetMatchedItemsIds(pullRequest.Title, pattern));

                                await AzureTableStorage.AddMergedWorkItems(ids);

                                var userSubscribedForMerges = await AzureTableStorage.GetUsersBySubscription(TableStorageSubscriptionColumns.SubscribedForRepositoryMerges);
                                foreach (var user in userSubscribedForMerges)
                                {
                                    await MessageComposer.SendMessageToUser(user.RowKey, user.ConversationId, $"Merged {pullRequest.Title} opened by {pullRequest.User.Username}");
                                }
                            }
                            
                            if (pullRequestData.Status == PullRequestStatus.Updated)
                            {
                                var userSubscribedForUpdates = await AzureTableStorage.GetUsersBySubscription(TableStorageSubscriptionColumns.SubscribedForRepositoryUpdates);
                                foreach (var user in userSubscribedForUpdates)
                                {
                                    await MessageComposer.SendMessageToUser(user.RowKey, user.ConversationId, $"Updated {pullRequest} opened by {pullRequest.User.Username}");
                                }
                            }

                            // TODO: send a message to the user in some interval, not on every bot run
                            if (pullRequestData.Status == PullRequestStatus.RequiresReview)
                            {
                                foreach (var gitUser in pullRequest.RequestedReviewers)
                                {
                                    // check if the user is subscribed
                                    var userSubscribedForReviewRequests = userSubscribedForRequestedReviews.FirstOrDefault(u => u.GitUsername == gitUser.Username);
                                    if (userSubscribedForReviewRequests != null)
                                    {
                                        await MessageComposer.SendMessageToUser(userSubscribedForReviewRequests.RowKey, userSubscribedForReviewRequests.ConversationId, $"Your review is requested for {pullRequest.Title} opened by {pullRequest.User.Username}. {pullRequest.HtmlUrl}");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    await MessageComposer.SendMessageToUser("29:1Zg8q4E-_qtiNekcsFmWdbaih5zkCNlg4hbxMXwRFO2gbpaaLJTNDd-ifz5NYV30WV1NWvHL1lHPz-Wzg6wNt8A", "a:1FF_jJrHwxIBqzPMx4yLVjtoCVcD6o5Dkh7xqetx-rYDHSxmSmtPYMHsvC3W_qBkEVHVDOd9sewyfFYdYvGtW7kTKLQjUq7r3Cdhc5RcIqYGQbradXpl9i1mMAxPNroUD", "No pull request were merged or updated");
                }
            }
            catch (Exception ex)
            {
                await MessageComposer.SendMessageToUser("29:1Zg8q4E-_qtiNekcsFmWdbaih5zkCNlg4hbxMXwRFO2gbpaaLJTNDd-ifz5NYV30WV1NWvHL1lHPz-Wzg6wNt8A", "a:1FF_jJrHwxIBqzPMx4yLVjtoCVcD6o5Dkh7xqetx-rYDHSxmSmtPYMHsvC3W_qBkEVHVDOd9sewyfFYdYvGtW7kTKLQjUq7r3Cdhc5RcIqYGQbradXpl9i1mMAxPNroUD", ex.ToString());

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    await MessageComposer.SendMessageToUser("29:1Zg8q4E-_qtiNekcsFmWdbaih5zkCNlg4hbxMXwRFO2gbpaaLJTNDd-ifz5NYV30WV1NWvHL1lHPz-Wzg6wNt8A", "a:1FF_jJrHwxIBqzPMx4yLVjtoCVcD6o5Dkh7xqetx-rYDHSxmSmtPYMHsvC3W_qBkEVHVDOd9sewyfFYdYvGtW7kTKLQjUq7r3Cdhc5RcIqYGQbradXpl9i1mMAxPNroUD", ex.ToString());
                }

                throw new Exception();
            }
        }

        private static IEnumerable<string> GetMatchedItemsIds(string text, string pattern)
        {
            var regex = new Regex(pattern);
            var ids = new List<string>();
            var matches = Regex.Matches(text, pattern);

            foreach (Match match in matches)
            {
                ids.Add(match.Groups[1].Value);
            }

            return ids;
        }
    }
}