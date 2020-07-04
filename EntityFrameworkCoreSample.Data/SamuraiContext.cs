using EntityFrameworkCoreSample.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCoreSample.Data
{
    public class SamuraiContext:DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<Battle> Battles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(ConsoleLoggerFacory)
#if DEBUG
                .EnableSensitiveDataLogging() //Newxt to other sensitive data logs queries parmaters. Never use on production
#endif
                .UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SamuraiAppData");
        }
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Specification of many-to-many relation in DB
            modelBuilder.Entity<SamuraiBattle>().HasKey(s => new { s.SamuraiId, s.BattleId });
            //This way we prevent direct interaction with horses (that would have been possible if Horses was a DbSet).
            //It is possible to set up Horse the way that its properties will be included in Samurai table (rather than is separate "Horse" table)
            modelBuilder.Entity<Horse>().ToTable("Horses");
        }

        //In ASP.Net Core logger is built in
        public static readonly ILoggerFactory ConsoleLoggerFacory = LoggerFactory.Create(builder =>
      {
          builder
            .AddFilter((category, level) =>   category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
            .AddConsole();
      });
    }
}

