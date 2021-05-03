using Microsoft.EntityFrameworkCore;
using ProjektWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektWebAPI.Data
{
    public class GeoMessageDbContext : DbContext
    {
        public GeoMessageDbContext(DbContextOptions<GeoMessageDbContext> options)
            : base(options)
        {

        }
        public DbSet<GeoMessage> GeoMessages {get;set;}
    }
}
