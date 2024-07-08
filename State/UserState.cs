using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScraperBot.State
{
    internal enum UserState
    {
        OnStart,
        OnGreeting,
        OnStackChoosing,
        OnGradeChoosing,
        OnEnd
    }
}
