using JobScraperBot.DAL;
using JobScraperBot.DAL.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JobScraperBot.Tests.Helpers
{
    public sealed class TestDbContextFactory : IDbContextFactory<JobScraperBotContext>, IDisposable
    {
        private readonly DbContextOptions<JobScraperBotContext> options;
        private readonly SqliteConnection connection;

        public TestDbContextFactory()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
            this.connection.Open();
            this.options = new DbContextOptionsBuilder<JobScraperBotContext>()
                .UseSqlite(this.connection)
                .Options;

            using var dbContext = new JobScraperBotContext(this.options);
            dbContext.Database.EnsureCreated();

            SeedData(dbContext);
        }

        public JobScraperBotContext CreateDbContext()
        {
            return new JobScraperBotContext(this.options);
        }

        public Task<JobScraperBotContext> CreateDbContextAsync()
        {
            return Task.FromResult(this.CreateDbContext());
        }

        public void Dispose()
        {
            this.connection?.Dispose();
        }

        private static void SeedData(JobScraperBotContext context)
        {
            var dotnet = new WorkStack() { Id = Guid.NewGuid(), StackName = ".net" };
            var frontend = new WorkStack() { Id = Guid.NewGuid(), StackName = "java" };
            var java = new WorkStack() { Id = Guid.NewGuid(), StackName = "full_stack" };
            var fullstack = new WorkStack() { Id = Guid.NewGuid(), StackName = "full_stack" };
            var python = new WorkStack() { Id = Guid.NewGuid(), StackName = "python" };

            var trainee = new Grade() { Id = Guid.NewGuid(), GradeName = "trainee_intern" };
            var junior = new Grade() { Id = Guid.NewGuid(), GradeName = "junior" };
            var middle = new Grade() { Id = Guid.NewGuid(), GradeName = "middle" };
            var senior = new Grade() { Id = Guid.NewGuid(), GradeName = "senior" };
            var teamlead = new Grade() { Id = Guid.NewGuid(), GradeName = "team_lead" };
            var head = new Grade() { Id = Guid.NewGuid(), GradeName = "head_chief" };

            var office = new JobKind() { Id = Guid.NewGuid(), KindName = "office" };
            var remote = new JobKind() { Id = Guid.NewGuid(), KindName = "remote" };
            var officeOrRemote = new JobKind() { Id = Guid.NewGuid(), KindName = "office_or_remote" };

            var daily = new MessageIntervalEntity() { Id = Guid.NewGuid(), Interval = "daily" };
            var oneceInTwoDays = new MessageIntervalEntity() { Id = Guid.NewGuid(), Interval = "once_in_two_days" };
            var weekly = new MessageIntervalEntity() { Id = Guid.NewGuid(), Interval = "weekly" };

            long firstChatID = 578150968L;
            long secondChatID = 678150968L;

            var firstSubscription = new Subscription()
            {
                Id = Guid.NewGuid(),
                ChatId = firstChatID,
                MessageInterval = daily,
                NextUpdate = new DateOnly(2024, 12, 12),
                Time = new TimeOnly(16, 0),
            };

            var secondSubscription = new Subscription()
            {
                Id = Guid.NewGuid(),
                ChatId = secondChatID,
                MessageInterval = oneceInTwoDays,
                NextUpdate = new DateOnly(2024, 12, 13),
                Time = new TimeOnly(17, 0),
            };

            var firstSubStngs = new SubscriptionSettings()
            {
                Id = Guid.NewGuid(),
                Subscription = firstSubscription,
                Stack = dotnet,
                Grade = junior,
                JobKind = officeOrRemote,
            };

            var secondSubStngs = new SubscriptionSettings()
            {
                Id = Guid.NewGuid(),
                Subscription = secondSubscription,
                Stack = python,
                Grade = junior,
                JobKind = null,
            };

            context.Stacks.AddRange(
                dotnet,
                frontend,
                java,
                fullstack,
                python);

            context.Grades.AddRange(
                trainee,
                junior,
                middle,
                senior,
                teamlead,
                head);

            context.JobKinds.AddRange(
                office,
                remote,
                officeOrRemote);

            context.MessageIntervals.AddRange(
                daily,
                oneceInTwoDays,
                weekly);

            context.Subscriptions.AddRange(firstSubscription, secondSubscription);
            context.SubscriptionSettings.AddRange(firstSubStngs, secondSubStngs);

            context.HiddenVacancies.AddRange(
                new HiddenVacancy() { Id = Guid.NewGuid(), ChatId = firstChatID, Link = "https://robota.ua/company14732167/vacancy10138807" },
                new HiddenVacancy() { Id = Guid.NewGuid(), ChatId = firstChatID, Link = "https://jobs.dou.ua/companies/infopulse/vacancies/287240/" },
                new HiddenVacancy() { Id = Guid.NewGuid(), ChatId = secondChatID, Link = "https://robota.ua/company14732167/vacancy10138807" });

            context.SaveChanges();
        }
    }
}
