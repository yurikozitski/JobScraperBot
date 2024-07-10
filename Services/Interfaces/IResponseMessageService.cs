using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IResponseMessageService
    {
        string GetResponseMessage(UserState state, UserSettings userSettings);
    }
}
