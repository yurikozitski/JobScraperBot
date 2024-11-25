using JobScraperBot.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace JobScraperBot.DAL
{
    public class JobScraperBotContext : DbContext
    {
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<SubscriptionSettings> SubscriptionSettings { get; set; }

        public DbSet<MessageInterval> MessageIntervals { get; set; }

        public DbSet<Stack> Stacks { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<Type> Types { get; set; }

        public JobScraperBotContext(DbContextOptions<JobScraperBotContext> options) : base(options)
        {
        }
    }
}
