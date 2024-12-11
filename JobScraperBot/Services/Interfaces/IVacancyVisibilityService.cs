using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    public interface IVacancyVisibilityService
    {
        Task HandleVacancyVisibilityAsync(ITelegramBotClient botClient, Update update);

        //void MarkVacancyAsVisible(Update update);
    }
}
