using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektWebAPI.Data
{
    public class GeoMessageDbContext : IdentityDbContext<User>
    {
        public GeoMessageDbContext(DbContextOptions<GeoMessageDbContext> options)
            : base(options)
        {

        }
        public DbSet<GeoMessage> GeoMessages {get;set;}
        public DbSet<User> Users { get; set; }

        public async Task SeedDatabase(UserManager<User> userManager)
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();
            

            var messageTest = new GeoMessage
            {
                Latitude = 30.3,
                Longitude = 25.1,
                Message = "Test"
            };
            var user = new User
            { UserName = "Test1", FirstName = "testnamn", LastName = "TestEfternamn" };
            
            await userManager.CreateAsync(user);
            await AddAsync(messageTest);
            await SaveChangesAsync();
        }
    }
}
