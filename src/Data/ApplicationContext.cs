using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coffee_Machine.Models;
using Microsoft.EntityFrameworkCore;

namespace Coffee_Machine.Data
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<Coffee> Coffee { get; set; }
        public DbSet<Coin> Coin { get; set; }

        // cannot later be updated 
        //public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=coffee_machine.db");
        }
    }
}
