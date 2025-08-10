using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities;

namespace ShoesStore.Routers;



public class FiltersRouter
{
    public static void Map(RouteGroupBuilder apiRoutes)
    {

        apiRoutes.MapGet("/categories", async (StoreDBContext context) =>
        {
            try
            {
                var categories = await context.Categories.Where(c => c.Products != null && c.Products.Count != 0).Select(c => new { c.CategoryUuid, c.Name }).ToListAsync();
                var successResponse = new
                {
                    success = true,
                    categories
                };
                return Results.Json(successResponse, statusCode: 200);
            }
            catch (Exception e)
            {
                return Results.InternalServerError(e.Message);
            }
        });




        apiRoutes.MapGet("/vendors", async (StoreDBContext context) =>
        {
            try
            {
                var vendors = await context.Vendors.Where(v => v.Products != null && v.Products.Count != 0).Select(v => new { v.VendorUuid, v.Name }).ToListAsync();
                var successResponse = new
                {
                    success = true,
                    vendors
                };
                return Results.Json(successResponse, statusCode: 200);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Results.InternalServerError(e.Message);
            }
        });



        apiRoutes.MapGet("/filter", async (StoreDBContext context, int? price, string? vendors, string? categories, int page) =>
        {
            try
            {
                int productsPerPage = 8;
                int offset = page - 1;


                var filterQuery = context.Products.AsQueryable<ShoesStore.Entities.Models.Product>();
                if (price != null)

                {
                    if (price is int)
                    {
                        filterQuery = filterQuery.Where(p => p.Price > price);
                    }
                    else
                    {
                        var errorResponse = new
                        {
                            success = false,
                            message = "The price is not an integer"
                        };
                        return Results.Json(errorResponse, statusCode: 400);
                    }

                }

                if (vendors != null && vendors.Trim() != "")
                {

                    string[] vendorsArr = vendors.Split(',');
                    bool isVendorsValid = true;
                    foreach (var vendor in vendorsArr)
                    {
                        if (!Guid.TryParse(vendor, out _))
                        {
                            isVendorsValid = false;
                            break;
                        }

                    }

                    if (isVendorsValid)
                    {
                        filterQuery = filterQuery.Where(p => vendorsArr.Contains(p.Vendor.VendorUuid));
                    }
                    else
                    {
                        var errorResponse = new
                        {
                            success = false,
                            message = "The vendors passed are in a wrong format"
                        };
                        return Results.Json(errorResponse, statusCode: 400);
                    }

                }

                if (categories != null && categories.Trim() != "")
                {

                    string[] categoriesArr = categories.Split(',');
                    bool isCategoriesValid = true;
                    foreach (var category in categoriesArr)
                    {
                        if (!Guid.TryParse(category, out _))
                        {
                            isCategoriesValid = false;
                            break;
                        }

                    }

                    if (isCategoriesValid)
                    {
                        filterQuery = filterQuery.Where(p => categoriesArr.Contains(p.Category.CategoryUuid));
                    }
                    else
                    {
                        var errorResponse = new
                        {
                            success = false,
                            message = "The vendors passed are in a wrong format"
                        };
                        return Results.Json(errorResponse, statusCode: 400);
                    }

                }

                var filteredResultsCount = await filterQuery.Select(p => new
                {
                    p.ProductId,
                    p.ProductUuid,
                    p.Title,
                    p.Description,
                    p.Price,
                    Category = p.Category.Name,
                    Vendor = new { p.Vendor.VendorUuid, p.Vendor.Name },
                    Medias = p.Medias != null ? p.Medias.Select(m => new { m.Path }) : null,
                    Sizes = p.Sizes.Select(s => new { s.SizeNumber }),
                }).CountAsync();


                int productsPages = filteredResultsCount % productsPerPage > 0 ? filteredResultsCount / productsPerPage + 1 : filteredResultsCount / productsPerPage;

                Console.WriteLine(filteredResultsCount % productsPerPage);

                var filteredResults = await filterQuery.Select(p => new
                {
                    p.ProductId,
                    p.ProductUuid,
                    p.Title,
                    p.Description,
                    p.Price,
                    Page = page,
                    Category = p.Category.Name,
                    Vendor = new { p.Vendor.VendorUuid, p.Vendor.Name },
                    Medias = p.Medias != null ? p.Medias.Select(m => new { m.Path }) : null,
                    Sizes = p.Sizes.Select(s => new { s.SizeNumber }),
                }).Skip(offset * productsPerPage).Take(productsPerPage).ToListAsync();


                var successResponse = new
                {
                    success = true,
                    products = filteredResults,
                    pages = productsPages
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