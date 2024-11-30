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

        public async Task DeleteAsync(HiddenVacancy vacancy)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            var hiddenVacancy = await context.HiddenVacancies.FirstOrDefaultAsync(x => x.ChatId == vacancy.ChatId && x.Link == vacancy.Link);

            if (hiddenVacancy != null)
            {
                context.Remove(hiddenVacancy);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteByChatIdAsync(long chatId)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            await context.HiddenVacancies.Where(x => x.ChatId == chatId).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<HiddenVacancy>> GetByChatIdAsync(long chatId)
        {
            using var context = await this.contextFactory.CreateDbContextAsync();

            var hiddenVac = await context.HiddenVacancies.Where(x => x.ChatId == chatId).ToListAsync();
            return hiddenVac;
        }
    }
}
