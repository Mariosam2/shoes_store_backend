



using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using ShoesStore.Routers;


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

SeederRouter.Map(apiRoutes);
PaymentRouter.Map(apiRoutes);
FiltersRouter.Map(apiRoutes);

var productsRoutes = apiRoutes.MapGroup("/products");
ProductRouter.Map(productsRoutes);

app.MapRazorPages();



app.Run();
