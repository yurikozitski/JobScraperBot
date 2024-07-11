using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;

namespace JobScraperBot.Services.Implementations
{
    internal class ResponseMessageService : IResponseMessageService
    {
        public string GetResponseMessage(UserState state, UserSettings userSettings)
        {
            return state switch
            {
                UserState.OnGreeting => "Вас вітає бот і т.д",
                UserState.OnStackChoosing => "Виберіть стек",
                UserState.OnGradeChoosing => "Виберіть рівень",
                UserState.OnTypeChoosing => "Виберіть вид роботи",
                UserState.OnEnd => $"Налаштування завершено, ваша підписка: {userSettings}",
                _ => "щось пішло не так..."
            };
        }
    }
}
