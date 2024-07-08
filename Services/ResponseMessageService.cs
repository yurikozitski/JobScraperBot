using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobScraperBot.State;

namespace JobScraperBot.Services
{
    internal static class ResponseMessageService
    {
        public static string GetResponseMessage(UserState state, UserSettings userSettings)
        {
            return state switch
            {
                UserState.OnGreeting => "Вас вітає бот і т.д",
                UserState.OnStackChoosing => "Виберіть стек",
                UserState.OnGradeChoosing => "Виберіть рівень",
                UserState.OnEnd => $"Налаштування завершено, ваша підписка: {userSettings}",
                _ => "щось пішло не так..."
            };
        }
    }
}
