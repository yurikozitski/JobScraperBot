using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;

namespace JobScraperBot.Services.Implementations
{
    internal class ResultChoosingHandler : IResultChoosingHandler
    {
        public void HandleResult(string message, IUserStateMachine userState)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(userState);

            if (userState.State != UserState.OnResultChoosing)
                return;

            if (message.Equals("Отримати зараз", StringComparison.InvariantCultureIgnoreCase))
                userState.MoveNext();
        }
    }
}
