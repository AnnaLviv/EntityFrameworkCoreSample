using EntityFrameworkCoreSample.Data;
using EntityFrameworkCoreSample.Domain;
using System;
using System.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        private static SamuraiContext context = new SamuraiContext();
        private static void Main(string[] args)
        {
            //cheating approach for sandbox purposes
            context.Database.EnsureCreated();
            GetSamurais("Before adding:");
            AddSamurai();
            GetSamurais("After adding:");

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

        private static void GetSamurais (string text)
        {
            var samurais = context.Samurais.ToList();
            Console.WriteLine($"{text} Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
                Console.WriteLine(samurai.Name);
        }
    }
}
