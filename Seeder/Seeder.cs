
using ShoesStore.Entities;

using ShoesStore.Entities.Models;
using Bogus;



namespace ShoesStore.Seeder;





public class Seeder()
{
    private readonly int VendorsNum = 5;
    private readonly int ProductsNum = 20;


    public void VendorsSeed(StoreDBContext context)

    {

        try
        {


            for (int i = 0; i < VendorsNum; i++)
            {
                var vendorFaker = new Faker<Vendor>()
                .StrictMode(false).Rules((faker, vendor) =>
                {
                    vendor.VendorUuid = faker.Random.Uuid().ToString();
                    vendor.Name = faker.Company.CompanyName();


                });

                Vendor newVendor = vendorFaker.Generate();
                Console.Write(newVendor.VendorUuid + ",");
                context.Vendors.Add(newVendor);
                context.SaveChanges();


            }



        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }





    }
    public void ProductsSeed(StoreDBContext context)
    {

        try
        {



            for (int i = 0; i < ProductsNum; i++)
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

                context.Products.Add(newProduct);
                context.SaveChanges();

            }


        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }



    }

    public void MediaSeed(StoreDBContext context)
    {
        try
        {

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
                context.Media.Add(firstMedia);
                context.Media.Add(secondMedia);
                context.SaveChanges();


            }




        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }



    }





}