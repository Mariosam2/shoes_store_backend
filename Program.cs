



using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShoesStore.Entities;
using ShoesStore.Entities.Models;
using ShoesStore.Seeder;


var storeAllowedOrigins = "_storeAllowedOrigins";

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(9, 1, 0));
builder.Services
.AddDbContext<StoreDBContext>(options =>
{
    if (connectionString != null)
    {
        options.UseMySql(connectionString, serverVersion);

    }

});

builder.Services.AddRazorPages();

builder.Services.AddCors((options) =>
{
    options.AddPolicy(name: storeAllowedOrigins, (policy) =>
    {
        policy.WithOrigins("http://localhost:42000");

    });
});


var app = builder.Build();

app.UseCors(storeAllowedOrigins);

app.MapPost("/seed", async (StoreDBContext context) =>
{

    try
    {
        var seeder = new Seeder();
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
            seeder.VendorsSeed(context);
            seeder.ProductsSeed(context);
            seeder.MediaSeed(context);
            var successResponse = new
            {
                success = true,
                message = "database seeded"
            };

            return Results.Json(JsonConvert.SerializeObject(successResponse), statusCode: 201);

        }
        else
        {

            var dbAlreadySeeded = new
            {
                success = false,
                message = "database already seeded"
            };


            return Results.Json(JsonConvert.SerializeObject(dbAlreadySeeded), statusCode: 200);

        }



    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.InternalServerError(e.Message);
    }



});

app.MapGet("/products", async (StoreDBContext context) =>
{
    return await context.Products.ToListAsync();
});

app.MapRazorPages();

app.Run();
