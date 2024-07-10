using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IUserStateService
    {
        void UpdateUserSettings(long chatId, Update update);
    }
}
