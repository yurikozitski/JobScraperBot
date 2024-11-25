namespace JobScraperBot.DAL.Entities
{
    public class SubscriptionSettings : BaseEntity
    {
        public Guid SubscriptionId { get; set; }

        public Subscription Subscription { get; set; } = default!;

        public Stack Stack { get; set; } = default!;

        public Grade Grade { get; set; } = default!;

        public JobKind? Type { get; set; }
    }
}
