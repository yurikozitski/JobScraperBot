using JobScraperBot.DAL.Entities;
using JobScraperBot.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobScraperBot.DAL.Repositories
{
    public class SubscriptionDbRepository : ISubscriptionRepository
    {
        private readonly IDbContextFactory<JobScraperBotContext> contextFactory;

        public SubscriptionDbRepository(IDbContextFactory<JobScraperBotContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task AddAsync(Subscription entity)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            var workStack = await context.Stacks.FirstOrDefaultAsync(x => x.StackName == entity.SubscriptionSettings.Stack.StackName);
            var grade = await context.Grades.FirstOrDefaultAsync(x => x.GradeName == entity.SubscriptionSettings.Grade.GradeName);
            var jobKind = 
                entity.SubscriptionSettings.JobKind != null ? 
                await context.JobKinds.FirstOrDefaultAsync(x => x.KindName == entity.SubscriptionSettings.JobKind.KindName) : null;
            var mesInt = await context.MessageIntervals.FirstOrDefaultAsync(x => x.Interval == entity.MessageInterval.Interval);

            entity.SubscriptionSettings.Stack = workStack!;
            entity.SubscriptionSettings.Grade = grade!;
            entity.SubscriptionSettings.JobKind = jobKind;
            entity.MessageInterval = mesInt!;

            await context.Subscriptions.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Delete(Subscription entity)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByChatIdAsync(long chatId)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            await context.Subscriptions.Where(x => x.ChatId == chatId).ExecuteDeleteAsync();
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
