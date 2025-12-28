using Microsoft.EntityFrameworkCore;
using Common;

namespace OrdersService.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(a => a.Id);
        }
    }
}
