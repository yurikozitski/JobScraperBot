using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    internal class MenuHandler : IMenuHandler
    {
        public async Task HandleMenuAsync(ITelegramBotClient botClient, Message message, IUserStateMachine currentUserState)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(message.Text);

            if (message.Text == "/reset")
            {
                currentUserState.Reset();

                string subPath = Directory.GetCurrentDirectory() + "\\HiddenVacancies";

                if (Directory.Exists(subPath))
                {
                    string path = subPath + $"\\{message.Chat.Id}_hidden.txt";

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
                    currentUserState.SetState(UserState.OnEnd);
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
