﻿using EntityFrameworkCoreSample.Data;
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
            AddSamurai();
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

            await QueryUsingRawSqlAsync();
            Console.WriteLine("Press any key ...");
            Console.ReadKey();
        }

        private static async Task QueryUsingRawSqlAsync()
        {
            //Creates as IQueryable, so we still need an execution method.
            //We must use parameters to avoid SQL injection
            //Query can't contain related data
            //Can only be used for known entities
            var samurais = await context.Samurais.FromSqlRaw("Select * from Samurais").ToListAsync();

            var samuraisWithQuotes = await context.Samurais.FromSqlRaw("Select Id,Name,ClanId from Samurais")
                .Include(s=>s.Quotes)
                .ToListAsync();

            var specificName = "Ramses";
            var samuraisWithSpecificName = await context.Samurais.FromSqlInterpolated($"Select * from Samurais Where Name = {specificName}")
                .ToListAsync();
            //It is possible to execute stored procedures the same way

            //Queries/SPs that can be executed on a DB.
            //Returns # of affcted entries.
            var numberOfRowsAffected = await context.Database.ExecuteSqlInterpolatedAsync($"Delete from Samurais Where Name = {specificName}");
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

        private static void GetSamurais(string text)
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
            var samuraisLike = await context.Samurais.Where(samurai => EF.Functions.Like(samurai.Name, likeName)).ToListAsync();

            var containsName = "ams";
            var samuraisContains = await context.Samurais.Where(samurai => samurai.Name.Contains(containsName)).ToListAsync();
        }

        private static async Task RetrieveAndUpdateSamuraiAsync(string name)
        {
            var samurai = await context.Samurais.FirstOrDefaultAsync(samurai => samurai.Name.Equals(name));
            if (samurai != null)
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
            using (var newContextInstance = new SamuraiContext())
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
            using (var newContext = new SamuraiContext())
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
                .Where(samurai => samurai.Name.Equals("Ramses"))
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
            foreach (var quote in samurai.Quotes)
            {
                Console.WriteLine(quote.Text);
            }

            //LazyLoading is NOT OK. All Quotes will be retrieved and materialized, and then count will be calculated
            var quotesCount = samurai.Quotes.Count();
        }

        private static async Task FilteringWithRelatedDataAsync()
        {
            //we want to retrieve samurais with "challenge" quotes.
            //we don't want quotes, only such samurais
            var samurais = await context.Samurais
                .Where(samurai => samurai.Quotes.Any(quote => quote.Text.Contains("challenge")))
                .ToListAsync();
        }

        private static async Task ModifyingRelatedDataWhenTrackedAsync()
        {
            var samurai = await context.Samurais
                .Include(samurai => samurai.Quotes)
                .FirstOrDefaultAsync(samurai => samurai.Id.Equals(2));
            samurai.Quotes.Remove(samurai.Quotes[2]);
            samurai.Quotes[0].Text = "What's up?";
            await context.SaveChangesAsync();
        }

        private static async Task ModifyingRelatedDataWhenNotTrackedAsync()
        {
            var samurai = await context.Samurais
                .Include(samurai => samurai.Quotes)
                .FirstOrDefaultAsync(samurai => samurai.Id.Equals(2));
            var quote = samurai.Quotes[0];
            quote.Text += "What's up now?";

            using (var newContext = new SamuraiContext())
            {
                newContext.Entry(quote).State = EntityState.Modified;
                await newContext.SaveChangesAsync();
            }
        }

        private static async Task JoinBattleAndSamuraiAsync()
        {            
            var sbJoin = new SamuraiBattle { SamuraiId = 2, BattleId = 1 };
            //There is no DbSet SamuraiBattle. We cannot use context.SamuraiBattles.Add
            await context.AddAsync(sbJoin);
            await context.SaveChangesAsync();
        }

        private static async Task EnlistSamuraiIntoBattleAsync()
        {
            var battle = await context.Battles.FindAsync(1);
            battle.SamuraiBattles.Add(new SamuraiBattle { SamuraiId = 1 });
            await context.SaveChangesAsync();
        }

        private static async Task RemoveJoinBattleAndSamuraiAsync()
        {
            //It is recommended to retrieve table from DB and delete it. For simple tables like SamuraiBattle the following approach is acceptable
            var sbJoin = new SamuraiBattle { SamuraiId = 2, BattleId = 1 };
            context.Remove(sbJoin);
            await context.SaveChangesAsync();
        }

        private static async Task GetSamuraiWithBattlesAsync()
        {
            var samuraiWithBattles = await context.Samurais
                .Include(samurai => samurai.SamuraiBattles)
                .ThenInclude(samuraiBattle => samuraiBattle.Battle)
                .FirstOrDefaultAsync(samurai => samurai.Id.Equals(2));

            var samuraiWithBattlesCleaner = await context.Samurais
                .Where(samurai => samurai.Id.Equals(2))
                .Select(samurai => new { Samurai = samurai, Battles = samurai.SamuraiBattles.Select(samuraiBattle => samuraiBattle.Battle) })
                .FirstOrDefaultAsync();
        }

        private static async Task AddNewSamuraiWithHorseAsync()
        {
            var samurai = new Samurai { Name = "Simba" };
            samurai.Horse = new Horse { Name = "Pegas" };
            await context.Samurais.AddAsync(samurai);
            await context.SaveChangesAsync();
        }

        private static async Task AddNewHorseToSamuraiUsingIdAsync()
        {
            var horse = new Horse { Name = "Pegas", SamuraiId=1 };
            await context.AddAsync(horse);
            await context.SaveChangesAsync();
        }

        private static async Task ReplaceAHorseAsync()
        {
            //  var samurai = await context.Samurais.Include(samurai => samurai.Horse).FirstOrDefaultAsync();
            var samurai = await context.Samurais.FindAsync(12);
            samurai.Horse =  new Horse { Name = "Pegas" };
            //if samurai is in memory, or if samurai already ha as horse - exception will pop up
            await context.SaveChangesAsync();
        }

        private static async Task GetSamuraisWithHorseAsync()
        {
            var samurais = await context.Samurais.Include(s => s.Horse).ToListAsync();
        }

        private static async Task GetHorsesWithSamuraisAsync()
        {
            var horseWithoutSamurai = await context.Set<Horse>().FindAsync(1);

            var horseWithSamurai = await context.Samurais
                .Include(s => s.Horse)
                .FirstOrDefaultAsync(s => s.Horse.Id.Equals(1));

            var horsesWithSamurais = await context.Samurais
                .Where(s => s.Horse != null)
                .Select(s => new { Samurai = s, Horse = s.Horse })
                .ToListAsync();
        }

        private static async Task GetSamuraiWithClanAsync()
        {
            var samurai = await context.Samurais.Include(s => s.Clan).FirstOrDefaultAsync();
        }

        private static async Task GetClanWithSamurais()
        {
            var clan = await context.Clans.FindAsync(1);
            var samuraisForClan = await context.Samurais.Where(s => s.Clan.Id.Equals(3)).ToListAsync();
        }

    }
}