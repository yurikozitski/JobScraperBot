using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScraperBot.State
{
    internal interface IStateMachine<out T>
    {
        T State { get; }

        bool MoveNext();
    }
}
