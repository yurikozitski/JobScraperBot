using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    internal class UserStateService : IUserStateService
    {
        private readonly IUserStateStorage storage;

        public UserStateService(IUserStateStorage storage)
        {
            this.storage = storage;
        }

        public void UpdateUserSettings(long chatId, Update update)
        {
            if (!this.storage.StateStorage.TryGetValue(chatId, out _))
            {
                return;
            }

            UserState currentUserState = this.storage.StateStorage[chatId].State;

            if (currentUserState == UserState.OnStackChoosing)
            {
                this.storage.StateStorage[chatId].UserSettings.Stack = update.Message!.Text!;
            }

            if (currentUserState == UserState.OnGradeChoosing)
            {
                this.storage.StateStorage[chatId].UserSettings.Grade = update.Message!.Text!;
            }
        }
    }
}
