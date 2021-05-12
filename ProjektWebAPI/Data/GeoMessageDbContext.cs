using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjektWebAPI.Models;
using ProjektWebAPI.Models.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektWebAPI.Data
{
    public class GeoMessageDbContext : IdentityDbContext<IdentityUser>
    {
        public GeoMessageDbContext(DbContextOptions<GeoMessageDbContext> options)
            : base(options)
        {

        }
        public DbSet<GeoMessage> GeoMessages {get;set;}
        public DbSet<GeoMessageV2> GeoMessagesV2 { get; set; }
        public DbSet<User> User { get; set; }

        public static async Task Seed(IServiceProvider prov)
        {
            var context = prov.GetRequiredService<GeoMessageDbContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var userManager = prov.GetRequiredService<UserManager<User>>();

            await userManager.CreateAsync(
                new User
                {
                    FirstName ="Test",
                    LastName="TestLastname",
                    UserName="TestUser",
                    Email="testMail@gmail.com"

                },
                "QWEqwe123!!");
            
            
        }
    }
}

