using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    internal interface ISubscriptionWriter
    {
        Task WriteSubscriptionAsync(long chatId, string sbscrptnText, IUserStateMachine userState);
    }
}
