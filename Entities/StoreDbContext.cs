using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities.Models;




namespace ShoesStore.Entities;

public class StoreDBContext(DbContextOptions<StoreDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Media> Media { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
        .HasIndex(u => u.Username)
        .IsUnique();



        builder.Entity<Product>()
        .HasIndex(p => p.ProductUuid)
        .IsUnique();

        builder.Entity<Product>()
       .Property(p => p.Price)
       .HasPrecision(3, 2);



        builder.Entity<Order>()
        .HasIndex(o => o.OrderUuid)
        .IsUnique();

        builder.Entity<Vendor>()
        .HasIndex(v => v.VendorUuid)
        .IsUnique();
    }






}


