using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JobScraperBot.DAL.Design
{
    public class JobScraperBotContextFactory : IDesignTimeDbContextFactory<JobScraperBotContext>
    {
        public JobScraperBotContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<JobScraperBotContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("RemoteDb"));

            return new JobScraperBotContext(optionsBuilder.Options);
        }
    }
}