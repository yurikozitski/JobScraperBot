using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    public class UserStateService : IUserStateService
    {
        private readonly IUserStateStorage storage;
        private readonly ILogger<UserStateService> logger;

        public UserStateService(IUserStateStorage storage, ILogger<UserStateService> logger)
        {
            this.storage = storage;
            this.logger = logger;
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

        public async Task LoadUserSettings()
        {
            string path = Directory.GetCurrentDirectory() + "\\Subscriptions";

            if (Directory.Exists(path))
            {
                foreach (string filePath in Directory.EnumerateFiles(path, "*.txt"))
                {
                    try
                    {
                        string subscription = await System.IO.File.ReadAllTextAsync(filePath);
                        string[] subscriptionParams = subscription.Split(',');

                        string chatIdStr = System.IO.Path.GetFileName(filePath).Split('_')[0].Trim();
                        long chatId = long.Parse(chatIdStr);

                        UserSettings userSettings = new UserSettings()
                        {
                            Stack = subscriptionParams[2].Trim(),
                            Grade = subscriptionParams[3].Trim(),
                            Type = !string.IsNullOrWhiteSpace(subscriptionParams[4].Trim()) ? subscriptionParams[4].Trim() : null,
                        };

                        var userState = new UserStateMachine(userSettings);
                        userState.SetState(UserState.OnEnd);

                        this.storage.StateStorage.TryAdd(chatId, userState);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Error occured while loading user state from files");
                    }
                }
            }
        }
    }
}
