using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScraperBot.State
{
    internal class UserSettings
    {
        public string Stack { get; set; } = default!;

        public string Grade { get; set; } = default!;

        public override string ToString() =>
            $"{Stack}, {Grade}";
    }
}
