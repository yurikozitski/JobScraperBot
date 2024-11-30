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

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            return await context.Subscriptions
                .Include(x => x.SubscriptionSettings.Stack)
                .Include(x => x.SubscriptionSettings.Grade)
                .Include(x => x.SubscriptionSettings.JobKind)
                .Include(x => x.MessageInterval)
                .ToListAsync();
        }

        public async Task DeleteByChatIdAsync(long chatId)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            await context.Subscriptions.Where(x => x.ChatId == chatId).ExecuteDeleteAsync();
        }

        public async Task UpdateDateAsync(Subscription subscription)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            await context.Subscriptions.Where(x => x.ChatId == subscription.ChatId).ExecuteUpdateAsync(
                setters => setters.SetProperty(s => s.NextUpdate, subscription.NextUpdate));
        }
    }
}
