using JobScraperBot.DAL.Entities;

namespace JobScraperBot.DAL.Interfaces
{
    public interface IHiddenVacancyRepository : IRepository<HiddenVacancy>
    {
        public Task DeleteAsync(HiddenVacancy vacancy);

        Task<IEnumerable<HiddenVacancy>> GetByChatIdAsync(long chatId);
    }
}
