namespace JobScraperBot.DAL.Entities
{
    public class Grade : BaseEntity
    {
        public string GradeName { get; set; } = default!;

        public ICollection<SubscriptionSettings> SubscriptionSettings { get; set; } = new List<SubscriptionSettings>();
    }
}
