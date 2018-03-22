namespace Microsoft.Bot.ReinventionBot.Models
{
    public class Review
    {
        /// <summary>
        /// The state can be:
        ///     1) CHANGES_REQUESTED
        ///     2) APPROVED
        ///     3) COMMENTED
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The author of the review.
        /// </summary>
        public User User { get; set; }
    }
}