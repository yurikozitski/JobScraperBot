using JobScraperBot.Services;
using JobScraperBot.State;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("7448548753:AAEkSnA2KdnzTExqwgz_sguLJ3UJo2pp4hU", cancellationToken: cts.Token);
bot.StartReceiving(HandleUpdate, async (bot, ex, ct) => 
{ 
    Console.WriteLine(ex);
});

var me = await bot.GetMeAsync();
Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

Console.ReadLine();
cts.Cancel();

async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken ct)
{
    if (update.Message is null) return;
    if (update.Message.Text is null) return;

    var chatId = update.Message.Chat.Id;

    //IUserStateMachine? currentUserState;

    if (!UserStateStorage.StateStorage.TryGetValue(chatId, out IUserStateMachine? currentUserState))
    {
        currentUserState = new UserStateMachine(new UserSettings());
        UserStateStorage.StateStorage.TryAdd(chatId, currentUserState);
    }

    UserStateService.UpdateUserSettings(chatId, update);

    UserStateStorage.StateStorage[chatId].MoveNext();

    string responseMessage = ResponseMessageService.GetResponseMessage(UserStateStorage.StateStorage[chatId].State, 
        UserStateStorage.StateStorage[chatId].UserSettings);
    var responseButtons = ResponseKeyboardService.GetResponseButtons(UserStateStorage.StateStorage[chatId].State);

    await bot.SendTextMessageAsync(chatId, responseMessage,
        replyMarkup: responseButtons != null ? new ReplyKeyboardMarkup(responseButtons) { ResizeKeyboard = true } : new ReplyKeyboardRemove());

    if (UserStateStorage.StateStorage[chatId].State == UserState.OnEnd)
    {
        var vacancies = await VacancyService.GetVacanciesAsync(bot, chatId, UserStateStorage.StateStorage[chatId].UserSettings);
        await VacancyService.ShowVacanciesAsync(bot, chatId, vacancies);
    }
}
