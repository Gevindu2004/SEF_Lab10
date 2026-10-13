using CampusEats.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Api.Data;

public class CampusEatsDbContext : DbContext
{
    public CampusEatsDbContext(DbContextOptions<CampusEatsDbContext> options)
        : base(options)
    {
    }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed initial data if tables are created
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "Kottu Roti", Price = 750m, Category = "Mains", Available = true },
            new MenuItem { Id = 2, Name = "Fried Rice", Price = 850m, Category = "Mains", Available = true },
            new MenuItem { Id = 3, Name = "Veg Sub", Price = 600m, Category = "Snacks", Available = false },
            new MenuItem { Id = 4, Name = "Watalappan", Price = 350m, Category = "Dessert", Available = true },
            new MenuItem { Id = 5, Name = "Iced Milo", Price = 300m, Category = "Drinks", Available = true }
        );
    }
}
