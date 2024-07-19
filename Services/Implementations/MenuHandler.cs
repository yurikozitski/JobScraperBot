using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    internal class MenuHandler : IMenuHandler
    {
        private readonly IUserSubscriptionsStorage subscriptionsStorage;

        public MenuHandler(IUserSubscriptionsStorage userSubscriptionsStorage)
        {
            this.subscriptionsStorage = userSubscriptionsStorage;
        }

        public async Task HandleMenuAsync(ITelegramBotClient botClient, Message message, IUserStateMachine currentUserState)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(message.Text);

            if (message.Text == "/reset")
            {
                currentUserState.Reset();

                this.subscriptionsStorage.Subscriptions.Remove(message.Chat.Id, out _);

                string subPathHidden = Directory.GetCurrentDirectory() + "\\HiddenVacancies";
                string subPathSbcscr = Directory.GetCurrentDirectory() + "\\Subscriptions";

                if (Directory.Exists(subPathHidden))
                {
                    string path = subPathHidden + $"\\{message.Chat.Id}_hidden.txt";

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                if (Directory.Exists(subPathSbcscr))
                {
                    string path = subPathSbcscr + $"\\{message.Chat.Id}_subscription.txt";

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }

            if (message.Text == "/confirm")
            {
                if (currentUserState.State > UserState.OnGradeChoosing)
                {
                    currentUserState.SetState(UserState.OnResultChoosing - 1);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ви ще не можете завершити налаштування, треба вибрати хоча б стек та рівень");
                    var currentState = currentUserState.State;
                    currentUserState.SetState(--currentState);
                }
            }
        }
    }
}
