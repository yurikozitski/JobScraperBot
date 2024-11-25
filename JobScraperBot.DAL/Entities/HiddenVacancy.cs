namespace JobScraperBot.DAL.Entities
{
    public class HiddenVacancy : BaseEntity
    {
        public long ChatId { get; set; }

        public string Link { get; set; } = default!;
    }
}
