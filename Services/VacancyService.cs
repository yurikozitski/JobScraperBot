using JobScraperBot.State;
using JobScraperBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace JobScraperBot.Services
{
    internal static class VacancyService
    {
        private static HttpClient httpClient = new HttpClient();

        public static async Task<IEnumerable<Vacancy>> GetVacanciesAsync(ITelegramBotClient bot, long chatId, UserSettings userSettings)
        {
            string requestString = RequestStringServive.GetRequestString(userSettings);

            string response;

            try
            {
                response = await httpClient.GetStringAsync(requestString);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nCan't load vacancies!");
                await bot.SendTextMessageAsync(chatId, "Упс...Щось пішло не так.");
                throw;
            }
            catch (Exception)
            {
                await bot.SendTextMessageAsync(chatId, "Упс...Щось пішло не так.");
                throw;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var vacancyList = JsonSerializer.Deserialize<IEnumerable<Vacancy>>(response, options);

            return vacancyList ?? Enumerable.Empty<Vacancy>();
        }

        public static async Task ShowVacanciesAsync(ITelegramBotClient bot, long chatId, IEnumerable<Vacancy> vacancies)
        {
            if (!vacancies.Any())
            {
                await bot.SendTextMessageAsync(chatId, "Нажаль не знайшолося вакансій за вашим запитом");
                return;
            }

            foreach (var vacancy in vacancies)
            {
                string vacancyView = string.Join(Environment.NewLine,
          $"Вебсайт: {vacancy.WebSite}",
                $"Посилання: {vacancy.Link}",
                $"Назва: {vacancy.JobTitle}",
                $"Компанія: {vacancy.Company}",
                $"Зарплата: {vacancy.Salary ?? "не вказано"}",
                $"Тип: {vacancy.JobType ?? "не вказано"}",
                $"Локація: {vacancy.Location ?? "не вказано"}",
                $"Опис: {vacancy.Description ?? "не вказано"}",
                $"Дата: {vacancy.PublicationDate}");

                await bot.SendTextMessageAsync(chatId, vacancyView);
            }
        }
    }
}
