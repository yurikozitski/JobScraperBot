using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScraperBot.State
{
    internal static class UserStateStorage
    {
        public static ConcurrentDictionary<long, IUserStateMachine> StateStorage { get; }

        static UserStateStorage()
        {
            StateStorage = new ConcurrentDictionary<long, IUserStateMachine>();
        }
    }
}
