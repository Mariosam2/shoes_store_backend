
using ShoesStore.Entities;

using ShoesStore.Entities.Models;
using Bogus;

using Stripe;


namespace ShoesStore.Seeder;





public class Seeder()
{
    private readonly int VendorsNum = 5;
    private readonly int ProductsNum = 5;


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

        var productFaker = new Faker<Entities.Models.Product>()
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


            Entities.Models.Product newProduct = productFaker.Generate();
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET");
            //Console.WriteLine("{0} {1} {2}", newProduct.Title, newProduct.Price, newProduct.Description);


            var options = new ProductCreateOptions
            {
                Name = newProduct.Title,
                DefaultPriceData = new ProductDefaultPriceDataOptions
                {
                    //unitamount in cents, decimal doesnt work
                    UnitAmount = Convert.ToInt64(newProduct.Price * 100),
                    Currency = "eur"
                },
                Description = newProduct.Description,

            };

            var service = new ProductService();
            Stripe.Product product = await service.CreateAsync(options);
            newProduct.StripePriceId = product.DefaultPriceId;
            newProduct.StripeProductId = product.Id;

            await context.Products.AddAsync(newProduct);


        }

        return await context.SaveChangesAsync();







    }

    public async Task<int> MediaSeed(StoreDBContext context)
    {

        string[] modelOneMedias = ["model-1-front.webp", "model-1-top.webp", "model-1-back.webp"];
        string[] modelTwoMedias = ["model-2-front.webp", "model-2-top.webp", "model-2-back.webp"];
        var products = context.Products.ToList();


        for (int i = 0; i < products.Count; i++)
        {
            Random random = new();

            var randMedias = random.Next(1, 3) == 1 ? modelOneMedias : modelTwoMedias;


            var mediaProduct = await context.Products.FindAsync(i + 1);



            for (int j = 0; j < randMedias.Length; j++)
            {

                if (mediaProduct != null)
                {
                    string path = Environment.GetEnvironmentVariable("BASE_URL") + "/images/" + randMedias[j];
                    Console.WriteLine(path);
                    var newMedia = new Media { Path = path, ProductId = i + 1, Product = mediaProduct };
                    await context.Media.AddAsync(newMedia);
                }
            }







        }
        return await context.SaveChangesAsync();









    }





}