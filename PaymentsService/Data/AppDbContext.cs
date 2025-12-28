using Microsoft.EntityFrameworkCore;
using PaymentsService.Models;

namespace PaymentsService.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(a => a.Id);
            modelBuilder.Entity<Transaction>().HasKey(t => t.OrderId);
        }
    }
}