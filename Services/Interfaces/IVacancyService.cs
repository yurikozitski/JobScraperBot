using JobScraperBot.State;
using JobsScraper.BLL.Models;
using Telegram.Bot;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IVacancyService
    {
        Task<IEnumerable<Vacancy>> GetVacanciesAsync(ITelegramBotClient bot, long chatId, UserSettings userSettings);

        Task ShowVacanciesAsync(ITelegramBotClient bot, long chatId, IEnumerable<Vacancy> vacancies);
    }
}
