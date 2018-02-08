namespace ProactiveBot.Utilities
{
    using System.Text;
    using ProactiveBot.Models;

    public static class MessageComposer
    {
        public static string SubscriptionStatus(TeamsUser user)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("# **Subscription status**\n\n");
            sb.Append("***\n\n");

            if (!user.SubscribedForRepositoryMerges && !user.SubscribedForRepositoryUpdates && !user.SubscribedForJenkinsBuildResults && !user.SubscribedForCiBuildResults)
            {
                sb.Append("You are not subscribed to any notifications.");
            }
            else
            {
                sb.Append(string.Format("* Iris repository merges by merge bot: {0}ubscribed\n\n", user.SubscribedForRepositoryMerges ? "S" : "Not s"));
                sb.Append(string.Format("* Iris repository updates by merge bot: {0}ubscribed\n\n", user.SubscribedForRepositoryUpdates ? "S" : "Not s"));
                sb.Append(string.Format("* Your Jenkins builds: {0}ubscribed\n\n", user.SubscribedForJenkinsBuildResults ? "S" : "Not s"));
                sb.Append(string.Format("* CI builds: {0}ubscribed\n\n", user.SubscribedForCiBuildResults ? "S" : "Not s"));
            }            

            return sb.ToString();
        }
    }
}