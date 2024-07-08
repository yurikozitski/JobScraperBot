using JobScraperBot.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace JobScraperBot.Services
{
    internal static class UserStateService
    {
        public static void UpdateUserSettings(long chatId, Update update)
        {
            if (!UserStateStorage.StateStorage.TryGetValue(chatId, out _))
            {
                return;
            }

            UserState currentUserState = UserStateStorage.StateStorage[chatId].State;

            if (currentUserState == UserState.OnStackChoosing)
            {
                UserStateStorage.StateStorage[chatId].UserSettings.Stack = update.Message!.Text!;
            }

            if (currentUserState == UserState.OnGradeChoosing)
            {
                UserStateStorage.StateStorage[chatId].UserSettings.Grade = update.Message!.Text!;
            }
        }
    }
}
