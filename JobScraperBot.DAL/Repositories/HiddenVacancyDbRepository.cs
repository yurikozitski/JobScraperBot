using JobScraperBot.DAL.Entities;
using JobScraperBot.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobScraperBot.DAL.Repositories
{
    public class HiddenVacancyDbRepository : IHiddenVacancyRepository
    {
        private readonly IDbContextFactory<JobScraperBotContext> contextFactory;

        public HiddenVacancyDbRepository(IDbContextFactory<JobScraperBotContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task AddAsync(HiddenVacancy entity)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            await context.HiddenVacancies.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Delete(HiddenVacancy entity)
        {
            using var context = this.contextFactory.CreateDbContext();

            var vacancy = context.HiddenVacancies.FirstOrDefault(x => x.ChatId == entity.ChatId && x.Link == entity.Link);

            if (vacancy != null)
            {
                context.Remove(vacancy);
                context.SaveChanges();
            }
        }

        public async Task DeleteByChatIdAsync(long chatId)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            await context.HiddenVacancies.Where(x => x.ChatId == chatId).ExecuteDeleteAsync();
        }

        public Task DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<HiddenVacancy>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HiddenVacancy>> GetByChatIdAsync(long chatId)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            var hiddenVac = await context.HiddenVacancies.Where(x => x.ChatId == chatId).ToListAsync();
            return hiddenVac;
        }

        public Task<HiddenVacancy> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(HiddenVacancy entity)
        {
            throw new NotImplementedException();
        }
    }
}
