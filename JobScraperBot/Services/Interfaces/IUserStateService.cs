using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    public interface IUserStateService
    {
        void UpdateUserSettings(long chatId, Update update);
    }
}
