namespace JobScraperBot.DAL.Entities
{
    public class JobKind : BaseEntity
    {
        public string TypeName { get; set; } = default!;

        public ICollection<SubscriptionSettings> SubscriptionSettings { get; set; } = new List<SubscriptionSettings>();
    }
}
