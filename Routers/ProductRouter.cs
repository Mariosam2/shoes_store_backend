using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities;

namespace ShoesStore.Routers;

public class ProductRouter
{
    public static void Map(RouteGroupBuilder productsRoutes)
    {

        productsRoutes.MapGet("/", async (StoreDBContext context, int page) =>
        {
            try
            {
                int productsPerPage = 8;
                int productsCount = await context.Products.CountAsync();
                int productsPages = productsCount % productsPerPage > 0 ? productsCount / productsPerPage + 1 : productsCount / productsPerPage;
                int offset = page - 1;
                var products = await context.Products.Where(p => p.ProductId > offset * productsPerPage).Select(p => new
                {
                    p.ProductUuid,
                    p.Title,
                    p.Description,
                    p.Price,
                    Page = page,
                    Category = p.Category.Name,
                    Vendor = new { p.Vendor.VendorUuid, p.Vendor.Name },
                    Medias = p.Medias != null ? p.Medias.Select(m => new { m.Path }) : null,
                    Sizes = p.Sizes.Select(s => new { s.SizeNumber }),
                }
                ).Take(productsPerPage).ToListAsync();

                var response = new
                {
                    success = true,
                    products,
                    pages = productsPages

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
                    Category = p.Category.Name,
                    Vendor = new { p.Vendor.VendorUuid, p.Vendor.Name },
                    Medias = p.Medias != null ? p.Medias.Select(m => new { m.Path }) : null,
                    Sizes = p.Sizes.Select(s => new { s.SizeNumber }),
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


        productsRoutes.MapGet("/search", async (StoreDBContext context, string query = "") =>
        {
            try
            {
                var searchedProducts = await context.Products.Where(p => p.Title.Contains(query))
                    .Select(p => new
                    {
                        p.ProductUuid,
                        p.Title,
                        p.Price,
                        Category = p.Category.Name,
                        Image = p.Medias != null ? p.Medias.Select(m => new { m.Path }).First().Path : null,
                        Sizes = p.Sizes.Select(s => new { s.SizeNumber }),
                    })
                    .Take(5).ToListAsync();


                var successResponse = new
                {
                    success = true,
                    results = new
                    {
                        products = searchedProducts,
                    }
                };

                return Results.Json(successResponse, statusCode: 200);


            }
            catch (Exception e)
            {
                return Results.InternalServerError(e.Message);

            }
        });


    }
}