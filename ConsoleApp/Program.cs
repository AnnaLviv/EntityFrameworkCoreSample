using EntityFrameworkCoreSample.Data;
using EntityFrameworkCoreSample.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        private static SamuraiContext context = new SamuraiContext();
        private static async Task Main(string[] args)
        {
            //cheating approach for sandbox purposes
            context.Database.EnsureCreated();
            //GetSamurais("Before adding:");
            //AddSamurai();
            //GetSamurais("After adding:");
            //InserMultipleSamurais();
            //InsertVariousTypes();
            //await QueryFiltersAsync();
            //await RetrieveAndUpdateSamuraiAsync("Regina");
            //  await RetrieveAndDeleteSamuraisAsync(3);
            //await RemoveVariousTypesAsync();

            await AddBattleAsync();
            await QueryAndUpdateBattleDicsonnected();
            Console.WriteLine("Press any key ...");
            Console.ReadKey();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Ramses" };
            context.Samurais.Add(samurai);
            //code triggeres saving results in DB
            context.SaveChanges();
        }

        private static void InserMultipleSamurais()
        {
            var samuraiOne = new Samurai { Name = "Regina" };
            var samuraiTwo = new Samurai { Name = "Rayna" };
            var samuraiThree = new Samurai { Name = "Benthe" };
            var samuraiFour = new Samurai { Name = "Dobbie" };
            //batching by EF is generated with more than 4 commands. Otherwise it is slower to execute commands sequentually
            context.Samurais.AddRange(samuraiOne, samuraiTwo, samuraiThree, samuraiFour);
            context.SaveChanges();
        }

        private static void InsertVariousTypes()
        {
            var samurai = new Samurai { Name = "DonaldDuck" };
            var clan = new Clan { ClanName = "Disney" };
            context.AddRange(samurai, clan);
            context.SaveChanges();
        }

        private static void GetSamurais (string text)
        {
            var samurais = context.Samurais.ToList();
            Console.WriteLine($"{text} Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
                Console.WriteLine(samurai.Name);
        }

        private static async Task QueryFiltersAsync()
        {
            var name = "Ramses";
            //EF optimizes SQL quesry for EF requests
            var samurais = await context.Samurais.Where(samurai => samurai.Name.Equals(name)).ToListAsync();

            //Both queries below will give the same result. "Contains" will be converted into "Like" in the background SQL query anyway
            var likeName = "%ams%";
            var samuraisLike = await context.Samurais.Where(samurai => EF.Functions.Like( samurai.Name, likeName)).ToListAsync();

            var containsName = "ams";
            var samuraisContains = await context.Samurais.Where(samurai => samurai.Name.Contains(containsName)).ToListAsync();
        }

        private static async Task RetrieveAndUpdateSamuraiAsync(string name)
        {
            var samurai = await context.Samurais.FirstOrDefaultAsync(samurai => samurai.Name.Equals(name));
            if(samurai!=null)
            {
                samurai.Name += "San";
                await context.SaveChangesAsync();
            }

        }

        private static async Task FindSamuraiAsync(int id)
        {
            var samurai = await context.Samurais.FindAsync(id);
        }

        private static async Task RetrieveAndUpdateMultipleSamuraisAsync()
        {
            var samurais = await context.Samurais.Skip(1).Take(3).ToListAsync();
            samurais.ForEach(samurai => samurai.Name += "San");
            await context.SaveChangesAsync();
        }

        private static async Task RetrieveAndDeleteSamuraisAsync(int id)
        {
            var samurai = await context.Samurais.FindAsync(id);
            context.Samurais.Remove(samurai);
            await context.SaveChangesAsync();
        }

        private static async Task RemoveVariousTypesAsync()
        {
            var samurai = await context.Samurais.FindAsync(1);
            var clan = await context.Clans.FindAsync(1);
            context.RemoveRange(samurai, clan);
            await context.SaveChangesAsync();
        }

        private static async Task AddBattleAsync()
        {
            var battle = new Battle { Name = "Pacific Ocean 34 67" };
            await context.Battles.AddAsync(battle);
            await context.SaveChangesAsync();
        }

        private static async Task QueryAndUpdateBattleDicsonnected()
        {
            // Alternatively to AsNoTracking() for not tracking in disconnected db we can specify 
            // ChangeTracker.QueryTrackingBehavior =  QueryTrackingBehavior.NoTracking
            // in SamuraiContext constructor. Then context by default will not track for changes.
            var battle = await context.Battles.AsNoTracking().FirstOrDefaultAsync();
            battle.EndDate = new DateTime(1875, 11, 21);
            using(var newContextInstance = new SamuraiContext())
            {
                 newContextInstance.Battles.Update(battle);
                 await newContextInstance.SaveChangesAsync();
            }
        }
    }
}
