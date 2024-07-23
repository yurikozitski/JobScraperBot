﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScraperBot.Services.Interfaces
{
    internal interface ISubscriptionsService
    {
        Task ReadFromFilesAsync(CancellationToken token);

        Task SendMessagesAsync(CancellationToken token);
    }
}
