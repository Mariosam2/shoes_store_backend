using Microsoft.EntityFrameworkCore;
using ShoesStore.Entities;
using ShoesStore.Entities.Models;
using ShoesStore.Types;
using Stripe;
using Stripe.Checkout;

namespace ShoesStore.Routers;

public class PaymentRouter
{


    public static void Map(RouteGroupBuilder apiRoutes)
    {
        apiRoutes.MapPost("/create-checkout-session", async (StoreDBContext context, HttpRequest request) =>
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


                    string domain = Environment.GetEnvironmentVariable("CLIENT_URL") ?? "http://localhost:5089";

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





        apiRoutes.MapPost("/create-order", async (StoreDBContext context, HttpRequest request) =>
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            //StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET");
            var stripeEvent = EventUtility.ConstructEvent(json,
                        request.Headers["Stripe-Signature"], Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET"));
            if (stripeEvent.Type == "charge.succeeded")
            {
                Console.WriteLine(stripeEvent.Data.Object);
                if (stripeEvent.Data.Object is Stripe.Charge stripeObj
                && stripeObj.BillingDetails.Name != null
                && stripeObj.BillingDetails.Address.Line1 != null
                && stripeObj.BillingDetails.Address.City != null
                && stripeObj.BillingDetails.Address.Country != null
                && stripeObj.BillingDetails.Email != null)
                {
                    var newOrder = new Order
                    {
                        CreatedAt = DateTime.Now,
                        OrderUuid = new Guid().ToString(),
                        CustomerName = stripeObj.BillingDetails.Name,
                        Address = stripeObj.BillingDetails.Address.Line1 +
                   ", " + stripeObj.BillingDetails.Address.City +
                   ", " + stripeObj.BillingDetails.Address.Country,
                        CustomerEmail = stripeObj.BillingDetails.Email,
                        Amount = Convert.ToDecimal(stripeObj.Amount),
                        ChargeID = stripeObj.Id,
                        PaymentIntentID = stripeObj.PaymentIntent.Id,
                        PaymentMethodID = stripeObj.PaymentMethod
                    };

                    await context.Orders.AddAsync(newOrder);
                    await context.SaveChangesAsync();

                    var successResponse = new
                    {
                        success = true,
                        message = "order created"
                    };
                    return Results.Json(successResponse, statusCode: 200);
                }
                else
                {
                    var errorResponse = new
                    {
                        success = false,
                        message = "customer details missing"
                    };

                    return Results.Json(errorResponse, statusCode: 400);
                }




            }

            var wrongEventResponse = new { success = false, message = "wrong event type" };
            return Results.Json(wrongEventResponse, statusCode: 200);
        });



    }
}