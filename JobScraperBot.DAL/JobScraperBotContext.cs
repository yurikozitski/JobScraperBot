using JobScraperBot.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraperBot.DAL
{
    public class JobScraperBotContext : DbContext
    {
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<SubscriptionSettings> SubscriptionSettings { get; set; }

        public DbSet<MessageIntervalEntity> MessageIntervals { get; set; }

        public DbSet<WorkStack> Stacks { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<JobKind> JobKinds { get; set; }

        public DbSet<HiddenVacancy> HiddenVacancies { get; set; }

        public JobScraperBotContext(DbContextOptions<JobScraperBotContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkStack>(b =>
            {
                b.HasData(
                    new WorkStack() { Id = Guid.NewGuid(), StackName = ".net" },
                    new WorkStack() { Id = Guid.NewGuid(), StackName = "front_end" },
                    new WorkStack() { Id = Guid.NewGuid(), StackName = "java" },
                    new WorkStack() { Id = Guid.NewGuid(), StackName = "full_stack" },
                    new WorkStack() { Id = Guid.NewGuid(), StackName = "python" });
            });

            modelBuilder.Entity<Grade>(b =>
            {
                b.HasData(
                    new Grade() { Id = Guid.NewGuid(), GradeName = "trainee_intern" },
                    new Grade() { Id = Guid.NewGuid(), GradeName = "junior" },
                    new Grade() { Id = Guid.NewGuid(), GradeName = "middle" },
                    new Grade() { Id = Guid.NewGuid(), GradeName = "senior" },
                    new Grade() { Id = Guid.NewGuid(), GradeName = "team_lead" },
                    new Grade() { Id = Guid.NewGuid(), GradeName = "head_chief" });
            });

            modelBuilder.Entity<JobKind>(b =>
            {
                b.HasData(
                    new JobKind() { Id = Guid.NewGuid(), KindName = "office" },
                    new JobKind() { Id = Guid.NewGuid(), KindName = "remote" },
                    new JobKind() { Id = Guid.NewGuid(), KindName = "office_or_remote" });
            });

            modelBuilder.Entity<MessageIntervalEntity>(b =>
            {
                b.HasData(
                    new MessageIntervalEntity() { Id = Guid.NewGuid(), Interval = "daily" },
                    new MessageIntervalEntity() { Id = Guid.NewGuid(), Interval = "once_in_two_days" },
                    new MessageIntervalEntity() { Id = Guid.NewGuid(), Interval = "weekly" });
                    
            });
        }
    }
}

