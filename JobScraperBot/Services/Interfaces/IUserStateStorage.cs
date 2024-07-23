using System.Collections.Concurrent;
using JobScraperBot.State;

namespace JobScraperBot.Services.Interfaces
{
    internal interface IUserStateStorage
    {
        ConcurrentDictionary<long, IUserStateMachine> StateStorage { get; }
    }
}
