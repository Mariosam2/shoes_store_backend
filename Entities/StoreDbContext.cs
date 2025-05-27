using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Bogus;

namespace ShoesStore.Entities;

public class StoreDBContext(DbContextOptions<StoreDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Media> Media { get; set; }


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


    public void UserAdminSeed(StoreDBContext context)
    {
        try
        {
            var admin = context.Users.Where(user => user.Username == "admin");

            Console.WriteLine("admin already exists");

        }
        catch (Exception e)
        {
            if (e is ArgumentNullException)
            {
                var newAdmin = new User { Username = "admin", Password = "admin", Role = "admin" };
                context.Users.Add(newAdmin);
                context.SaveChanges();
            }
        }

    }

    public void VendorsSeed(StoreDBContext context)
    {
        List<Vendor> newVendors = [];
        for (int i = 0; i < 5; i++)
        {
            var vendorFaker = new Faker<Vendor>()
            .StrictMode(false).Rules((faker, vendor) =>
            {
                vendor.VendorUuid = faker.Random.Uuid().ToString();
                vendor.Name = faker.Company.CompanyName();


            });

            Vendor newVendor = vendorFaker.Generate();
            newVendors.Add(newVendor);

        }

        context.Vendors.AddRange(newVendors);

        context.SaveChanges();


    }
    public void ProductsSeed(StoreDBContext context)
    {
        List<Product> newProducts = [];
        for (int i = 0; i < 20; i++)
        {
            var productFaker = new Faker<Product>()
            .StrictMode(false).Rules((faker, product) =>

            {
                var vendors = context.Vendors.ToList();
                product.ProductUuid = faker.Random.Uuid().ToString();
                product.Title = faker.Commerce.ProductName();
                product.Description = faker.Commerce.ProductDescription();
                product.Vendor = faker.PickRandom<Vendor>(vendors);




            });

            Product newProduct = productFaker.Generate();
            newProducts.Add(newProduct);


        }

        context.Products.AddRange(newProducts);

        context.SaveChanges();


    }

    public void MediaSeed(StoreDBContext context)
    {
        List<Media> newMedias = [];
        var products = context.Products.ToList();
        for (int i = 0; i < products.Count; i++)
        {
            var mediaFaker = new Faker<Media>()
                    .StrictMode(false)
                    .Rules((faker, media) =>
                    {

                        media.Path = faker.Image.PicsumUrl();
                        media.Product = products[i];
                    });

            Media firstMedia = mediaFaker.Generate();
            Media secondMedia = mediaFaker.Generate();

            newMedias.AddRange([firstMedia, secondMedia]);
        }

        context.Media.AddRange(newMedias);
        context.SaveChanges();





    }




}


