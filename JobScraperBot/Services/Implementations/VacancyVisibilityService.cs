using JobScraperBot.DAL.Entities;
using JobScraperBot.DAL.Interfaces;
using JobScraperBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services.Implementations
{
    public class VacancyVisibilityService : IVacancyVisibilityService
    {
        private readonly IHiddenVacancyRepository hiddenVacancyRepository;

        public VacancyVisibilityService(IHiddenVacancyRepository hiddenVacancyRepository)
        {
            this.hiddenVacancyRepository = hiddenVacancyRepository;
        }

        public async Task HandleVacancyVisibilityAsync(ITelegramBotClient botClient, Update update)
        {
            ArgumentNullException.ThrowIfNull(update);
            ArgumentNullException.ThrowIfNull(update.CallbackQuery);
            ArgumentNullException.ThrowIfNull(update.CallbackQuery.Data);

            var hiddenVacancies = await this.hiddenVacancyRepository.GetByChatIdAsync(update.CallbackQuery.Message!.Chat.Id);
            var hiddenVacancy = hiddenVacancies.FirstOrDefault(x => x.Link == update.CallbackQuery.Data);

            if (hiddenVacancy != null)
            {
                await this.hiddenVacancyRepository.DeleteAsync(hiddenVacancy);

                await botClient.EditMessageReplyMarkup(
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
                await this.hiddenVacancyRepository.AddAsync(new HiddenVacancy()
                {
                    ChatId = update.CallbackQuery.Message!.Chat.Id,
                    Link = update.CallbackQuery.Data,
                });

                await botClient.EditMessageReplyMarkup(
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
