using JobScraperBot.DAL.Entities;

namespace JobScraperBot.DAL.Interfaces
{
    public interface IHiddenVacancyRepository : IRepository<HiddenVacancy>
    {
        Task<IEnumerable<HiddenVacancy>> GetByChatIdAsync(long chatId);

        Task DeleteByChatIdAsync(long chatId);
    }
}
