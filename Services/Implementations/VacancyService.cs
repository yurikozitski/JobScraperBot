using System.IO.Compression;
using System.Text;
using System.Text.Json;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;
using JobsScraper.BLL.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobScraperBot.Services.Implementations
{
    internal class VacancyService : IVacancyService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRequestStringService requestStringService;

        public VacancyService(IRequestStringService requestStringService, IHttpClientFactory httpClientFactory)
        {
            this.requestStringService = requestStringService;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(ITelegramBotClient bot, long chatId, UserSettings userSettings)
        {
            string requestString = this.requestStringService.GetRequestString(userSettings);

            string response;

            //try
            //{
            //    response = await this.httpClientFactory.CreateClient().GetStringAsync(requestString);
            //}
            //catch (HttpRequestException)
            //{
            //    Console.WriteLine("\nCan't load vacancies!");
            //    await bot.SendTextMessageAsync(chatId, "Упс...Щось пішло не так.");
            //    throw;
            //}
            //catch (Exception)
            //{
            //    await bot.SendTextMessageAsync(chatId, "Упс...Щось пішло не так.");
            //    throw;
            //}

            using StreamReader r = new StreamReader("testData.json");
            response = r.ReadToEnd();

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

                try
                {
                    await bot.SendTextMessageAsync(chatId, vacancyView, replyMarkup: new InlineKeyboardMarkup(GetVacancyButton(vacancy.Link)));
                    Console.WriteLine(vacancy.Link.Length);
                }
                catch
                {
                    Console.WriteLine($"Failed vacanncy link length: {vacancy.Link.Length}");
                    //Console.WriteLine($"Failed vacanncy link length in base64: {Convert.ToBase64String(Zip(vacancy.Link)).Length}");
                    continue;
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
    }
}
