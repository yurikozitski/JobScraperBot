using JobScraperBot.DAL.Interfaces;
using JobScraperBot.Exceptions;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    public class MenuHandler : IMenuHandler
    {
        private readonly IUserSubscriptionsStorage subscriptionsStorage;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IHiddenVacancyRepository hiddenVacancyRepository;

        public MenuHandler(
            IUserSubscriptionsStorage userSubscriptionsStorage,
            ISubscriptionRepository subscriptionRepository,
            IHiddenVacancyRepository hiddenVacancyRepository)
        {
            this.subscriptionsStorage = userSubscriptionsStorage;
            this.subscriptionRepository = subscriptionRepository;
            this.hiddenVacancyRepository = hiddenVacancyRepository;
        }

        public async Task HandleMenuAsync(ITelegramBotClient botClient, Message message, IUserStateMachine currentUserState)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(message.Text);

            if (message.Text == "/reset")
            {
                try
                {
                    await this.subscriptionRepository.DeleteByChatIdAsync(message.Chat.Id);
                    await this.hiddenVacancyRepository.DeleteByChatIdAsync(message.Chat.Id);

                    this.subscriptionsStorage.Subscriptions.Remove(message.Chat.Id, out _);
                    currentUserState.Reset();
                }
                catch (Exception ex)
                {
                    throw new FailedOperationException(message.Chat.Id, ex.Message, ex);
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
                    await botClient.SendMessage(message.Chat.Id, "Ви ще не можете завершити налаштування, треба вибрати хоча б стек та рівень");
                    var currentState = currentUserState.State;
                    currentUserState.SetState(--currentState);
                }
            }
        }
    }
}
