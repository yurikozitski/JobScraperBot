using JobScraperBot.Services.Interfaces;

namespace JobScraperBot.Services.Implementations
{
    public class FileRemover : IFileRemover
    {
        public void RemoveFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
