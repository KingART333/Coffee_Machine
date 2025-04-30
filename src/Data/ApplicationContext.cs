using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using src.Models;
using Microsoft.EntityFrameworkCore;

namespace Coffee_Machine.Data
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<CoinTransaction> CoinTransactions { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<Coffee> Coffee { get; set; }
        public DbSet<Coin> Coin { get; set; }

        // cannot later be updated 
        //public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "coffee.db"));
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
