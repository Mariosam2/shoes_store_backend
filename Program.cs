



using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
.AddDbContext<StoreDBContext>(options =>
{
    if (connectionString != null)
    {
        options.UseMySQL(connectionString);

    }

    options.UseSeeding((context, _) =>
        {
            Console.WriteLine(context);
            if (context is StoreDBContext storeContext)
            {
                storeContext.UserAdminSeed(storeContext);
                storeContext.VendorsSeed(storeContext);
                storeContext.ProductsSeed(storeContext);
                storeContext.MediaSeed(storeContext);


            }
            else
            {
                Console.WriteLine("Error while seeding the database");
            }




        });



});


var app = builder.Build();


app.MapGet("/products", async (StoreDBContext context) =>
{
    return await context.Products.ToListAsync();
});

app.Run();
