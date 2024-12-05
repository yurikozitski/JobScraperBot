namespace JobScraperBot.DAL.Entities
{
    public class MessageIntervalEntity : BaseEntity
    {
        public string Interval { get; set; } = default!;

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
