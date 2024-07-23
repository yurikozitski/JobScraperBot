using System.Collections.Concurrent;
using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    public interface IUserStateStorage
    {
        ConcurrentDictionary<long, IUserStateMachine> StateStorage { get; }
    }
}
