using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities;
using ShoesStore.Entities.Models;
using ShoesStore.Seeder;
namespace ShoesStore.Routers;


public class SeederRouter
{
    public static void Map(RouteGroupBuilder apiRoutes)
    {

        apiRoutes.MapPost("/seed", async (StoreDBContext context) =>


        {

            try
            {
                var seeder = new Seed();
                //Seed the admin if not present
                var admin = context.Users.FirstOrDefault(user => user.Username == "admin");
                if (admin == null)
                {
                    var newAdmin = new User { Username = "admin", Password = "admin", Role = "admin" };
                    context.Users.Add(newAdmin);
                    await context.SaveChangesAsync();

                }

                //Seed the other data
                int vendorsCount = await context.Vendors.CountAsync();
                int productsCount = await context.Products.CountAsync();
                int mediaCount = await context.Media.CountAsync();

                if (vendorsCount == 0 && productsCount == 0 && mediaCount == 0)
                {
                    await seeder.VendorsSeed(context);
                    await seeder.CategoriesSeed(context);
                    await seeder.SizesSeed(context);
                    await seeder.ProductsSeed(context);
                    await seeder.MediaSeed(context);
                    var successResponse = new
                    {
                        success = true,
                        message = "database seeded"
                    };

                    return Results.Json(successResponse, statusCode: 201);

                }
                else
                {

                    var dbAlreadySeeded = new
                    {
                        success = false,
                        message = "database already seeded"
                    };


                    return Results.Json(dbAlreadySeeded, statusCode: 200);

                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Results.InternalServerError(e.Message);
            }



        });

    }
}