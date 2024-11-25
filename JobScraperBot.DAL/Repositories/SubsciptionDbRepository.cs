using JobScraperBot.DAL.Entities;
using JobScraperBot.DAL.Interfaces;

namespace JobScraperBot.DAL.Repositories
{
    public class SubsciptionDbRepository : ISubscriptionRepository
    {
        private readonly JobScraperBotContext context;

        public SubsciptionDbRepository(JobScraperBotContext context)
        {
            this.context = context;
        }

        public Task AddAsync(Subscription entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Subscription entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Subscription>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Subscription> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Subscription entity)
        {
            throw new NotImplementedException();
        }
    }
}
