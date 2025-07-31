using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using travelapp.Model;

namespace travelapp.Persistance.MsSql
{
    public class trDbContext : DbContext
    {
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Destination> Destinations { get; set; }

        public trDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=traveldb;Integrated Security=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Destination>().HasKey(d => new { d.airlineName, d.city });
            base.OnModelCreating(modelBuilder);
        }
    }
}
