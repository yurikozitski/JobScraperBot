using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    public interface IResultChoosingHandler
    {
        void HandleResult(string message, IUserStateMachine userState);
    }
}
