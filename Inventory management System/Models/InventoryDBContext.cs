using Microsoft.EntityFrameworkCore;

namespace Inventory_management_System.Models
{
    public class InventoryDBContext(DbContextOptions options):DbContext(options)
    {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            }

            public DbSet<Employee> Employees { get; set; }
            public virtual DbSet<BaseUser> Users { get; set; }
            public DbSet<Manager> Managers { get; set; }
            public DbSet<Item> Items { get; set; }
            public DbSet<Sale> Sales { get; set; }
            public DbSet<Audit_Log> Audit_logs { get; set; }


    }

}

