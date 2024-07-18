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
                UserState.OnResultChoosing => "Бажаєте отримати результати лише зараз чи додатково налаштувати періодичну підписку?",
                UserState.OnSubscriptionSetting => string.Join(
                    Environment.NewLine,
                    "Ви можете налаштувати таку періодичність повідомдень з вакансіями:",
                    "'щодня', 'через день', 'щотижня' та вказати час доби у форматі: 'Хвилини:Секунди'",
                    "Наприклад: 'щодня,18:00' або 'через день,07:38'."),
                UserState.OnEnd => $"Налаштування завершено, ваша підписка: {userSettings}",
                _ => "щось пішло не так..."
            };
        }
    }
}
