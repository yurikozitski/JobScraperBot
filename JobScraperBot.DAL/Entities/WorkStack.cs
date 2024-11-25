namespace JobScraperBot.DAL.Entities
{
    public class WorkStack : BaseEntity
    {
        public string StackName { get; set; } = default!;

        public ICollection<SubscriptionSettings> SubscriptionSettings { get; set; } = new List<SubscriptionSettings>();
    }
}
