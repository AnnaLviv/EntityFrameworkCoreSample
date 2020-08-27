using EntityFrameworkCoreSample.Data;
using EntityFrameworkCoreSample.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCoreSample.Tests
{
    [TestClass]
    public class ControllerIntegrationTests
    {
        private readonly WebApplicationFactory<SamuraiApi.Startup> factory;

        public ControllerIntegrationTests()
        {
            factory = new WebApplicationFactory<SamuraiApi.Startup>();
        }
        [TestMethod]
        public async Task CanInsertSamuraiIntoDatabase()
        {
            //Arrange
            var client = factory.CreateClient();
            //Act
            var response = await client.GetAsync("api/samurais");
            //var builder = new DbContextOptionsBuilder();
            //builder.UseInMemoryDatabase("CanInsertSamurai");
            //using (var context = new SamuraiContext(builder.Options))
            //{
            //    var samurai = new Samurai();
            //    await context.Samurais.AddAsync(samurai);
            //    Assert.AreEqual(EntityState.Added, context.Entry(samurai).State);
            //}

        }
    }
}
