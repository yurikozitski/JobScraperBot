using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;

namespace JobScraperBot.Services.Implementations
{
    public class ResponseMessageService : IResponseMessageService
    {
        public string GetResponseMessage(UserState state, UserSettings userSettings)
        {
            return state switch
            {
                UserState.OnGreeting => string.Join(
                    Environment.NewLine,
                    "Вас вітає бот для пошуку роботи в ІТ.",
                    "Підберіть необхідні для Вас налаштування",
                    "слідуючи подальшим інстукціям.",
                    "Ви можете використовувати такі команди меню:",
                    "/reset - скинути свої налаштування,",
                    "/confirm - завершити налаштування.",
                    "Введіть будь-що, щоб продовжити."),
                UserState.OnStackChoosing => "Виберіть стек",
                UserState.OnGradeChoosing => "Виберіть рівень",
                UserState.OnTypeChoosing => "Виберіть вид роботи",
                UserState.OnResultChoosing => "Бажаєте отримати результати лише зараз чи додатково налаштувати періодичну підписку?",
                UserState.OnSubscriptionSetting => string.Join(
                    Environment.NewLine,
                    "Ви можете налаштувати таку періодичність повідомдень з вакансіями:",
                    "'щодня', 'через день', 'щотижня' та вказати час доби у форматі: 'Години:Хвилини'",
                    "Наприклад: 'щодня,18:00' або 'через день,07:38'."),
                UserState.OnEnd => $"Налаштування завершено, ваша підписка: {userSettings}",
                _ => "щось пішло не так..."
            };
        }
    }
}
