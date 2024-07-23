using JobScraperBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services.Implementations
{
    public class VacancyVisibilityService : IVacancyVisibilityService
    {
        public async Task HandleVacancyVisibilityAsync(ITelegramBotClient botClient, Update update)
        {
            ArgumentNullException.ThrowIfNull(update);
            ArgumentNullException.ThrowIfNull(update.CallbackQuery);
            ArgumentNullException.ThrowIfNull(update.CallbackQuery.Data);

            string subPath = Directory.GetCurrentDirectory() + "\\HiddenVacancies";

            Directory.CreateDirectory(subPath);

            string[]? fileArray = null;
            string path = subPath + $"\\{update.CallbackQuery.Message?.Chat.Id}_hidden.txt";

            if (System.IO.File.Exists(path))
            {
                fileArray = await System.IO.File.ReadAllLinesAsync(path);

                if (fileArray.Contains(update.CallbackQuery.Data))
                {
                    fileArray = fileArray.Where(x => !x.Equals(update.CallbackQuery.Data)).ToArray();
                    await System.IO.File.WriteAllLinesAsync(path, fileArray);
                    await botClient.EditMessageReplyMarkupAsync(
                        update.CallbackQuery.Message!.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[][]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Більше не показувати", update.CallbackQuery.Data),
                            },
                        }));
                }
                else
                {
                    var fileList = fileArray.ToList();
                    fileList.Add(update.CallbackQuery.Data);
                    await System.IO.File.WriteAllLinesAsync(path, fileList);
                    await botClient.EditMessageReplyMarkupAsync(
                        update.CallbackQuery.Message!.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[][]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("❌Приховано", update.CallbackQuery.Data),
                            },
                        }));
                }
            }
            else
            {
                await System.IO.File.AppendAllTextAsync(path, update.CallbackQuery.Data + Environment.NewLine);
                await botClient.EditMessageReplyMarkupAsync(
                        update.CallbackQuery.Message!.Chat.Id,
                        update.CallbackQuery.Message.MessageId,
                        replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton[][]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("❌Приховано", update.CallbackQuery.Data),
                            },
                        }));
            }
        }
    }
}
