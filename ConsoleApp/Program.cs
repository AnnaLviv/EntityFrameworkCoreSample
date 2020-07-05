using EntityFrameworkCoreSample.Data;
using EntityFrameworkCoreSample.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            //await RetrieveAndDeleteSamuraisAsync(3);
            //await RemoveVariousTypesAsync();

            //await AddBattleAsync();
            //await QueryAndUpdateBattleDicsonnectedAsync();
            //await AddQuoteToExistingSamuraiNotTrackedAsync();
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

        private static async Task QueryAndUpdateBattleDicsonnectedAsync()
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

        private static async Task<int> InsertNewSamuraiWithQuoteAsync()
        {
            var samurai = new Samurai
            {
                Name = "Samurai With Qote",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "I will make it legendary"}
                }
            };
            await context.Samurais.AddAsync(samurai);
            await context.SaveChangesAsync();
            return samurai.Id;
        }

        private static async Task AddQuoteToExistingSamuraiNotTrackedAsync()
        {
            var samuraiWithQuoteId = await InsertNewSamuraiWithQuoteAsync();
            var samurai = await context.Samurais.FindAsync(samuraiWithQuoteId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Time for a new challenge"
            });
            using(var newContext = new SamuraiContext())
            {
                //newContext.Samurais.Update(samurai);
                //For performance Update can be replaced with Attach
                //Attach connects to object and sets its state to Unmodified
                //However EF will still see missing key and missing foreign key and set them up
                newContext.Samurais.Attach(samurai);
                await newContext.SaveChangesAsync();
            }
        }

        private static async Task EagerLoadingSamuraiWithQuotesAsync()
        {
            //EF core uses "Left Join" query to extract the data with Include().
            //Include() works only on collections of entities, not on a single entity.
            var samuraiWithQuotes = await context.Samurais
                .Where(samurai=>samurai.Name.Equals("Ramses"))
                .Include(samurai => samurai.Quotes)
              //  .Include(samurai=>samurai.Clan)  //We can inlcude multiple children
              //  .ThenInclude(quote=> quote.Child)    //ThenInclude can retrieve grandchildren
                .ToListAsync();
        }

        private static async Task ProjectSomePropertiesAsync()
        {
            //possible to create anonymous types, with aggregations
            //EF core can only track entities recognized by the DbContext model.
            //Anonymous types are not tracked. Entities that are properties of an anonymous type are tracked
            //var someProperties = await context.Samurais.Select(samurai => new { samurai.Id, samurai.Name, samurai.Quotes.Count }).ToListAsync();

            var samuraisWithChallengeQuotes = await context.Samurais
               .Select(samurai => new { Samurai = samurai, ChallengeQuotes = samurai.Quotes.Where(quote => quote.Text.Contains("challenge")) })
               .ToListAsync();
            var firstSamuraiWithChallengeQuotes = samuraisWithChallengeQuotes[0].Samurai.Name += "Challenging";

        }

        private static async Task ExplicitLoadingQuotesAsync()
        {
            var samurai = await context.Samurais.FirstOrDefaultAsync(samurai => samurai.Name.Equals("Ramses"));
            //We can call Load only for a single object
            //Profile to determine whether Linq query would be a better performance 
            await context.Entry(samurai).Collection(samuraiItem => samuraiItem.Quotes).LoadAsync();
            await context.Entry(samurai).Reference(samuraiItem => samuraiItem.Horse).LoadAsync();

            //With Query we can filter related data that yet to be loaded
            var challengeQuotes = await context.Entry(samurai)
                .Collection(samuraiItem => samuraiItem.Quotes)
                .Query()
                .Where(quote => quote.Text.Contains("challenge"))
                .ToListAsync();
        }

        private static async Task LazyLoadingAsync()
        {
            var samurai = await context.Samurais.FirstOrDefaultAsync(samurai => samurai.Name.Equals("Ramses"));
            //LazyLoading is OK. Only one command to retrieve all Quotes will be sent
            foreach(var quote in samurai.Quotes)
            {
                Console.WriteLine(quote.Text);
            }

            //LazyLoading is NOT OK. All Quotes will be retrieved and materialized, and then count will be calculated
            var quotesCount = samurai.Quotes.Count();
        }


    }
}
