namespace JobScraperBot.DAL.Entities
{
    public class Subscription : BaseEntity
    {
        public long ChatId { get; set; }

        public SubscriptionSettings SubscriptionSettings { get; set; } = default!;

        public Guid MessageIntervalId { get; set; }

        public MessageInterval MessageInterval { get; set; } = default!;

        public TimeOnly Time { get; set; }

        public DateTime NextUpdate { get; set; }
    }
}
