using EntityFrameworkCoreSample.Domain;
using Microsoft.EntityFrameworkCore;

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
            optionsBuilder.UseSqlServer(
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
    }
}

