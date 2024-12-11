using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    public interface IMessageValidator
    {
        bool IsMessageValid(string message, UserState userState);
    }
}
