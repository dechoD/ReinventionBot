using System.Collections.Generic;

namespace Microsoft.Bot.ReinventionBot.Models
{

    public class PullRequestInformation
    {
        public IEnumerable<PullRequest> PullRequests { get; set; }

        public PullRequestStatus Status { get; set; }
    }
}
