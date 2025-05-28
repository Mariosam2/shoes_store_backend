
using ShoesStore.Entities;

using ShoesStore.Entities.Models;
using Bogus;
using Bogus.Extensions;



namespace ShoesStore.Seeder;





public class Seeder()
{
    private readonly int VendorsNum = 5;
    private readonly int ProductsNum = 20;


    public async Task<int> VendorsSeed(StoreDBContext context)

    {


        var vendorFaker = new Faker<Vendor>()
        .StrictMode(false).Rules((faker, vendor) =>
        {
            vendor.VendorUuid = faker.Random.Uuid().ToString();
            vendor.Name = faker.Company.CompanyName();


        });


        for (int i = 0; i < VendorsNum; i++)
        {

            Vendor newVendor = vendorFaker.Generate();
            Console.Write(newVendor.VendorUuid + ",");
            await context.Vendors.AddAsync(newVendor);



        }

        return await context.SaveChangesAsync();








    }
    public async Task<int> ProductsSeed(StoreDBContext context)
    {

        var productFaker = new Faker<Product>()
        .StrictMode(false).Rules((faker, product) =>
        {
            var vendors = context.Vendors.ToList();
            product.ProductUuid = faker.Random.Uuid().ToString();
            product.Title = faker.Commerce.ProductName();
            product.Price = faker.Finance.Amount(10, 150, 2);
            product.Description = faker.Commerce.ProductDescription();
            product.Vendor = faker.PickRandom<Vendor>(vendors);





        });

        for (int i = 0; i < ProductsNum; i++)
        {


            Product newProduct = productFaker.Generate();

            await context.Products.AddAsync(newProduct);


        }

        return await context.SaveChangesAsync();







    }

    public async Task<int> MediaSeed(StoreDBContext context)
    {


        var products = context.Products.ToList();


        for (int i = 0; i < products.Count; i++)
        {

            var mediaFaker = new Faker<Media>()
            .StrictMode(false)
            .Rules((faker, media) =>
            {

                media.Path = faker.Image.PicsumUrl(250, 250);
                media.ProductId = i + 1;

            });

            Media firstMedia = mediaFaker.Generate();
            Media secondMedia = mediaFaker.Generate();
            await context.Media.AddAsync(firstMedia);
            await context.Media.AddAsync(secondMedia);



        }
        return await context.SaveChangesAsync();









    }





}