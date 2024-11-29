using JobScraperBot.DAL.Entities;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Interfaces
{
    public interface IUserStateService
    {
        void UpdateUserSettings(long chatId, Update update);

        void LoadUserSettingsIntoMemory(IEnumerable<Subscription> subscriptions);
    }
}
