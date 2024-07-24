using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScraperBot.Services.Interfaces
{
    public interface IFileRemover
    {
        void RemoveFile(string path);
    }
}
