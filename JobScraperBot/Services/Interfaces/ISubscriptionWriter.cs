using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    public interface ISubscriptionWriter
    {
        Task WriteSubscriptionAsync(long chatId, string sbscrptnText, IUserStateMachine userState);
    }
}
