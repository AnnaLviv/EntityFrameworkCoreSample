using EntityFrameworkCoreSample.Data;
using EntityFrameworkCoreSample.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCoreSample.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public async Task CanInsertSamuraiIntoDatabase()
        {
            using(var context = new SamuraiContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                var samurai = new Samurai();
                await context.Samurais.AddAsync(samurai);
                Debug.WriteLine($"Before save: {samurai.Id}");

                await context.SaveChangesAsync();
                Debug.WriteLine($"After save: {samurai.Id}");

                Assert.AreNotEqual(0, samurai.Id);
            }

        }
    }
}
