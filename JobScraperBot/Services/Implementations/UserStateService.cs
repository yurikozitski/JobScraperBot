using AutoMapper;
using JobScraperBot.DAL.Entities;
using JobScraperBot.Models;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    public class UserStateService : IUserStateService
    {
        private readonly IUserStateStorage storage;
        private readonly IMapper mapper;

        public UserStateService(
            IUserStateStorage storage,
            IMapper mapper)
        {
            this.storage = storage;
            this.mapper = mapper;
        }

        public void UpdateUserSettings(long chatId, Update update)
        {
            if (!this.storage.StateStorage.TryGetValue(chatId, out _) ||
                update == null ||
                update.Message == null ||
                update.Message.Text == null ||
                update.Message?.Text == "/reset" ||
                update.Message?.Text == "/confirm")
            {
                return;
            }

            UserState currentUserState = this.storage.StateStorage[chatId].State;

            if (currentUserState == UserState.OnStackChoosing)
            {
                this.storage.StateStorage[chatId].UserSettings.Stack = update.Message!.Text;
            }

            if (currentUserState == UserState.OnGradeChoosing)
            {
                this.storage.StateStorage[chatId].UserSettings.Grade = update.Message!.Text;
            }

            if (currentUserState == UserState.OnTypeChoosing)
            {
                this.storage.StateStorage[chatId].UserSettings.Type = update.Message!.Text;
            }
        }

        public void LoadUserSettingsIntoMemory(IEnumerable<Subscription> subscriptions)
        {
            foreach (var subscription in subscriptions)
            {
                var userSettings = this.mapper.Map<Subscription, SubscriptionInfo>(subscription).UserSettings;

                var userState = new UserStateMachine(userSettings);
                userState.SetState(UserState.OnEnd);
                this.storage.StateStorage.TryAdd(subscription.ChatId, userState);
            }
        }
    }
}
