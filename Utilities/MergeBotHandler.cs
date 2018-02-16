namespace ReinventionBot.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    using Models;
    using Newtonsoft.Json;

    public static class MergeBotHandler
    {
        public static async Task HandleMergeBotMessage(Activity activity)
        {
            try
            {
                var pullRequestsProperty = activity.Properties["pullrequests"];

                if (pullRequestsProperty.HasValues)
                {
                    PullRequestInformation[] pullRequests = JsonConvert.DeserializeObject<PullRequestInformation[]>(pullRequestsProperty.ToString());

                    var mergedPr = pullRequests.Where(pr => pr.Merged).FirstOrDefault();
                    if (mergedPr != null)
                    {
                        var pattern = @"\s+#(\d{6})";
                        var ids = new List<string>();

                        ids.AddRange(GetMatchedItemsIds(mergedPr.Description, pattern));
                        ids.AddRange(GetMatchedItemsIds(mergedPr.Title, pattern));

                        await AzureTableStorage.AddMergedWorkItems(ids);

                        var userSubscribedForMerges = await AzureTableStorage.GetUsersBySubscription(TableStorageSubscriptionColumns.SubscribedForRepositoryMerges);
                        foreach (var user in userSubscribedForMerges)
                        {
                            await MessageComposer.SendMessageToUser(user.RowKey, user.ConversationId, $"Merged {mergedPr.Name} opened by {mergedPr.AuthorLogin}");
                        }
                    }

                    var updatedPr = pullRequests.Where(pr => pr.Updated).FirstOrDefault();
                    if (updatedPr != null)
                    {
                        var userSubscribedForUpdates = await AzureTableStorage.GetUsersBySubscription(TableStorageSubscriptionColumns.SubscribedForRepositoryUpdates);
                        foreach (var user in userSubscribedForUpdates)
                        {
                            await MessageComposer.SendMessageToUser(user.RowKey, user.ConversationId, $"Updated {updatedPr.Name} opened by {updatedPr.AuthorLogin}");
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