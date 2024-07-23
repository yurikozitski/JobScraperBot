using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    public interface IResponseMessageService
    {
        string GetResponseMessage(UserState state, UserSettings userSettings);
    }
}
