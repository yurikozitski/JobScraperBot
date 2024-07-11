using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserStateStorage userStateStorage;
        private readonly IUserStateService userStateService;
        private readonly IResponseMessageService responseMessageService;
        private readonly IResponseKeyboardService responseKeyboardService;
        private readonly IVacancyService vacancyService;
        private readonly IMessageValidator messageValidator;
        private readonly IMenuHandler menuHandler;

        public UpdateHandler(
            IUserStateStorage userStateStorage,
            IUserStateService userStateService,
            IResponseMessageService responseMessageService,
            IResponseKeyboardService responseKeyboardService,
            IVacancyService vacancyService,
            IMessageValidator messageValidator,
            IMenuHandler menuHandler)
        {
            this.userStateStorage = userStateStorage;
            this.userStateService = userStateService;
            this.responseMessageService = responseMessageService;
            this.responseKeyboardService = responseKeyboardService;
            this.vacancyService = vacancyService;
            this.messageValidator = messageValidator;
            this.menuHandler = menuHandler;
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null || update.Message.Text == null) return;

            var chatId = update.Message.Chat.Id;

            IUserStateMachine? currentUserState;

            if (!this.userStateStorage.StateStorage.TryGetValue(chatId, out currentUserState))
            {
                currentUserState = new UserStateMachine(new UserSettings());
                this.userStateStorage.StateStorage.TryAdd(chatId, currentUserState);
            }

            if (!this.messageValidator.IsMessageValid(update.Message, currentUserState.State))
            {
                await botClient.SendTextMessageAsync(chatId, $"Команда '{update.Message.Text}' не валідна");
                return;
            }

            this.userStateService.UpdateUserSettings(chatId, update);

            await this.menuHandler.HandleMenuAsync(botClient, update.Message, currentUserState);

            currentUserState.MoveNext();

            string responseMessage = this.responseMessageService.GetResponseMessage(
                currentUserState.State,
                currentUserState.UserSettings);

            var responseButtons = this.responseKeyboardService.GetResponseButtons(currentUserState.State);

            await botClient.SendTextMessageAsync(
                chatId,
                responseMessage,
                replyMarkup: responseButtons != null ? new ReplyKeyboardMarkup(responseButtons) { ResizeKeyboard = true } : new ReplyKeyboardRemove());

            if (currentUserState.State == UserState.OnEnd)
            {
                var vacancies = await this.vacancyService.GetVacanciesAsync(botClient, chatId, currentUserState.UserSettings);
                await this.vacancyService.ShowVacanciesAsync(botClient, chatId, vacancies);
            }
        }
    }
}
