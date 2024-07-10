using System.Collections.Concurrent;
using JobScraperBot.Services.Interfaces;
using JobScraperBot.State;

namespace JobScraperBot.Services.Implementations
{
    internal class UserStateStorage : IUserStateStorage
    {
        public ConcurrentDictionary<long, IUserStateMachine> StateStorage { get; }

        public UserStateStorage()
        {
            this.StateStorage = new ConcurrentDictionary<long, IUserStateMachine>();
        }
    }
}
