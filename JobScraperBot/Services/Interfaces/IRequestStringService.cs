using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    public interface IRequestStringService
    {
        string GetRequestString(UserSettings userSettings);
    }
}
