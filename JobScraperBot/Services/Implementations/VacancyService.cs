using System.Text.Json;
using JobScraperBot.Exceptions;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using JobsScraper.BLL.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services.Implementations
{
    internal class VacancyService : IVacancyService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRequestStringService requestStringService;
        private readonly ILogger<VacancyService> logger;

        public VacancyService(
            IRequestStringService requestStringService,
            IHttpClientFactory httpClientFactory,
            ILogger<VacancyService> logger)
        {
            this.requestStringService = requestStringService;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(ITelegramBotClient bot, long chatId, UserSettings userSettings)
        {
            string requestString = this.requestStringService.GetRequestString(userSettings);

            string? response = default;

            try
            {
                response = await this.httpClientFactory.CreateClient().GetStringAsync(requestString);
            }
            catch (HttpRequestException ex)
            {
                await bot.SendTextMessageAsync(chatId, "Упс...Щось пішло не так.");
                throw new VacancyLoadException(ex.Message, requestString);
            }
            catch (Exception ex)
            {
                await bot.SendTextMessageAsync(chatId, "Упс...Щось пішло не так.");
                throw new VacancyLoadException(ex.Message, requestString);
            }

            //using StreamReader r = new StreamReader("testData.json");
            //response = r.ReadToEnd();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var vacancyList = JsonSerializer.Deserialize<IEnumerable<Vacancy>>(response, options);

            return vacancyList ?? Enumerable.Empty<Vacancy>();
        }

        public async Task ShowVacanciesAsync(ITelegramBotClient bot, long chatId, IEnumerable<Vacancy> vacancies)
        {
            if (!vacancies.Any())
            {
                await bot.SendTextMessageAsync(chatId, "Нажаль не знайшолося вакансій за вашим запитом");
                return;
            }

            foreach (var vacancy in vacancies)
            {
                string vacancyView = string.Join(
                    Environment.NewLine,
                    $"Вебсайт: {vacancy.WebSite}",
                    $"Посилання: {vacancy.Link}",
                    $"Назва: {vacancy.JobTitle}",
                    $"Компанія: {vacancy.Company}",
                    $"Зарплата: {vacancy.Salary ?? "не вказано"}",
                    $"Тип: {vacancy.JobType ?? "не вказано"}",
                    $"Локація: {vacancy.Location ?? "не вказано"}",
                    $"Опис: {vacancy.Description ?? "не вказано"}",
                    $"Дата: {vacancy.PublicationDate}");

                string trimmedLink = GetTrimmedLink(vacancy.Link);

                var hiddenVacanciesLinks = await GetHiddenVacanciesLinksAsync(chatId);

                if (hiddenVacanciesLinks != null &&
                    hiddenVacanciesLinks.Contains(trimmedLink))
                {
                    continue;
                }

                try
                {
                    await bot.SendTextMessageAsync(chatId, vacancyView, replyMarkup: new InlineKeyboardMarkup(GetVacancyButton(trimmedLink)));
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, $"Can't show vacancy: {vacancy.Link}");
                }
            }
        }

        private static InlineKeyboardButton[][] GetVacancyButton(string vacancyLink)
        {
            return new InlineKeyboardButton[][]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Більше не показувати", vacancyLink),
                },
            };
        }

        private static string GetTrimmedLink(string link)
        {
            if (link.Length <= 64)
                return link;

            return link.Substring(link.Length - 64);
        }

        private static async Task<string[]?> GetHiddenVacanciesLinksAsync(long chatId)
        {
            string[]? fileArray = null;
            string path = Directory.GetCurrentDirectory() + "\\HiddenVacancies" + $"\\{chatId}_hidden.txt";

            if (System.IO.File.Exists(path))
            {
                fileArray = await System.IO.File.ReadAllLinesAsync(path);
            }

            return fileArray;
        }
    }
}
