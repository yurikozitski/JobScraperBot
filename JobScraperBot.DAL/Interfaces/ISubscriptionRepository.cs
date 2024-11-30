using JobScraperBot.DAL.Entities;

namespace JobScraperBot.DAL.Interfaces
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Task<IEnumerable<Subscription>> GetAllAsync();

        Task UpdateDateAsync(Subscription subscription);
    }
}
