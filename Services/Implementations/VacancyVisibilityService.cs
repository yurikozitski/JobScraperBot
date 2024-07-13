using JobScraperBot.Services.Interfaces;
using Telegram.Bot.Types;

namespace JobScraperBot.Services.Implementations
{
    internal class VacancyVisibilityService : IVacancyVisibilityService
    {
        public async Task HandleVacancyVisibilityAsync(Update update)
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
                }
                else
                {
                    var fileList = fileArray.ToList();
                    fileList.Add(update.CallbackQuery.Data);

                    await System.IO.File.WriteAllLinesAsync(path, fileList);
                }
            }
            else
            {
                await System.IO.File.AppendAllTextAsync(path, update.CallbackQuery.Data + Environment.NewLine);
            }
        }
    }
}
