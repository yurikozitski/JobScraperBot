using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IRequestStringService
    {
        string GetRequestString(UserSettings userSettings);
    }
}
