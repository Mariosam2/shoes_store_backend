using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities.Models;
using Bogus;

namespace ShoesStore.Entities;

public class StoreDBContext(DbContextOptions<StoreDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Media> Media { get; set; }


    public void UsersSeed(StoreDBContext context)


    {

        for (int i = 0; i < 10; i++)
        {
            var userFaker = new Faker<User>()
            .StrictMode(false).Rules((faker, user) =>
            {
                user.Username = faker.Internet.UserName();
                user.CreatedAt = DateTime.Now;


            });
            var newUser = userFaker.Generate();
            context.Users.Add(newUser);


        }

        context.SaveChanges();






    }
    public void VendorsSeed()
    {
        var userFaker = new Faker<Vendor>()
        .StrictMode(false).Rules((faker, vendor) =>
        {
            vendor.VendorUuid = faker.Random.Uuid().ToString();
            vendor.Name = faker.Company.CompanyName();


        });


    }
    public void ProductsSeed(StoreDBContext context)
    {
        var userFaker = new Faker<Product>()
        .StrictMode(false).Rules(async (faker, product) =>

        {
            var Vendors = await context.Vendors.ToListAsync();
            product.ProductUuid = faker.Random.Uuid().ToString();
            product.Title = faker.Commerce.ProductName();
            product.Description = faker.Commerce.ProductDescription();
            product.Vendor = faker.PickRandom<Vendor>(Vendors);




        });


    }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();


        builder.Entity<Product>()
        .HasIndex(p => p.ProductUuid)
        .IsUnique();

        builder.Entity<Order>()
        .HasIndex(o => o.OrderUuid)
        .IsUnique();

        builder.Entity<Vendor>()
        .HasIndex(v => v.VendorUuid)
        .IsUnique();
    }
}


