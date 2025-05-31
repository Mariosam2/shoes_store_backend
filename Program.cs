



using Microsoft.EntityFrameworkCore;
using Stripe;
using ShoesStore.Entities;
using ShoesStore.Entities.Models;
using ShoesStore.Seeder;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using ShoesStore.Types;
using Stripe.Checkout;

string storeAllowedOrigins = "_storeAllowedOrigins";

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

builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
      {
          c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shoes Store API", Description = "Store API", Version = "v1" });
      });
}


builder.Services.AddRazorPages();


builder.Services.AddCors((options) =>
{
    options.AddPolicy(name: storeAllowedOrigins, builder =>
    {
        builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors(storeAllowedOrigins);


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "images")),
    RequestPath = "/images"
});
app.UseSwagger();

app.UseSwaggerUI(c =>
 {
     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API V1");
 });


var apiRoutes = app.MapGroup("/api/v1");





apiRoutes.MapPost("/seed", async (StoreDBContext context) =>
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
            await seeder.VendorsSeed(context);
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
        //Console.WriteLine(e);
        return Results.InternalServerError(e.Message);
    }



});





apiRoutes.MapPost("create-checkout-session", async (StoreDBContext context, HttpRequest request) =>
{

    try
    {
        var reqBody = await request.ReadFromJsonAsync<CheckoutItems>();
        if (reqBody != null)
        {
            var items = reqBody.Items;
            var products = await context.Products.ToArrayAsync();
            var itemsUuid = items.Select(i => i.ProductUUID);
            var stripePriceIds = products.Where(p => itemsUuid.Contains(p.ProductUuid)).Select(p => p.StripePriceId).ToArray();
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET");


            var checkoutLineItems = new List<SessionLineItemOptions>();

            for (int i = 0; i < stripePriceIds.Length; i++)

            {

                var priceId = stripePriceIds[i];
                var quantity = items[i].Quantity;

                var sessionLineItem = new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = quantity,
                };

                checkoutLineItems.Add(sessionLineItem);
            }


            string domain = Environment.GetEnvironmentVariable("CLIENT_URL");

            var options = new SessionCreateOptions
            {
                UiMode = "custom",
                ReturnUrl = domain + "/after-payment?session_id={CHECKOUT_SESSION_ID}",
                LineItems = checkoutLineItems,
                Mode = "payment",
                PaymentMethodConfiguration = Environment.GetEnvironmentVariable("STRIPE_PAYMENT_CONFIG")
            };

            var service = new SessionService();
            Session session = service.Create(options);
            //onsole.WriteLine(session);

            var successResponse = new
            {
                success = true,
                clientSecret = session.ClientSecret
            };

            return Results.Json(successResponse, statusCode: 200);
        }
        var errorResponse = new
        {
            success = false,
            message = "Wrong request body format"
        };
        return Results.Json(errorResponse, statusCode: 400);


    }
    catch (Exception e)
    {
        return Results.InternalServerError(e.Message);
    }

});

var productsRoutes = apiRoutes.MapGroup("/products");

productsRoutes.MapGet("/", async (StoreDBContext context, int page) =>
{
    try
    {
        int offset = page - 1;
        var products = await context.Products.Where(p => p.ProductId > offset * 10).Select(p => new
        {
            p.ProductUuid,
            p.Title,
            p.Description,
            p.Price,
            Vendor = new { p.Vendor.VendorUuid, p.Vendor.Name },
            Medias = p.Medias != null ? p.Medias.Select(m => new { m.Path }) : null,
        }
        ).Take(10).ToListAsync();

        var response = new
        {
            success = true,
            products

        };

        return Results.Json(response, statusCode: 200);
    }
    catch (Exception e)
    {
        return Results.InternalServerError(e.Message);
    }


});

productsRoutes.MapGet("/{productUUID}", async (string productUUID, StoreDBContext context) =>
{
    try
    {
        var findProduct = await context.Products.Where(p => p.ProductUuid == productUUID).Select(p => new
        {

            p.ProductUuid,
            p.Title,
            p.Description,
            p.Price,
            Vendor = new { p.Vendor.VendorUuid, p.Vendor.Name },
            Medias = p.Medias != null ? p.Medias.Select(m => new { m.Path }) : null,
        }).FirstAsync();

        var successResponse = new
        {
            success = true,
            product = findProduct
        };

        return Results.Json(successResponse, statusCode: 200);
    }
    catch (Exception e)
    {
        var errResponse = new
        {
            success = false,
        };
        if (e.Message == "Sequence contains no elements.")
        {
            return Results.Json(new { success = false, message = "Product not found" }, statusCode: 404);
        }
        return Results.InternalServerError(e.Message);


    }


});


app.MapRazorPages();



app.Run();
