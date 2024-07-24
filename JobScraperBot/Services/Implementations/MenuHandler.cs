using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    public class MenuHandler : IMenuHandler
    {
        private readonly IUserSubscriptionsStorage subscriptionsStorage;
        private readonly IFileRemover fileRemover;

        public MenuHandler(
            IUserSubscriptionsStorage userSubscriptionsStorage,
            IFileRemover fileRemover)
        {
            this.subscriptionsStorage = userSubscriptionsStorage;
            this.fileRemover = fileRemover;
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

                string pathHidden = subPathHidden + $"\\{message.Chat.Id}_hidden.txt";
                string pathSbcscr = subPathSbcscr + $"\\{message.Chat.Id}_subscription.txt";

                this.fileRemover.RemoveFile(pathHidden);
                this.fileRemover.RemoveFile(pathSbcscr);
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
