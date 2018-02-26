using System.Collections.Generic;

namespace ReinventionBot.Models
{

    public class PullRequestInformation
    {
        public IEnumerable<PullRequest> PullRequests { get; set; }

        public PullRequestStatus Status { get; set; }
    }
}
