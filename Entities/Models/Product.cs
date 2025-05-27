namespace ShoesStore.Entities.Models;

using Bogus;
using Microsoft.EntityFrameworkCore;

public class Product
{
    public int ProductId { get; set; }
    public required string ProductUuid { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public required Vendor Vendor { get; set; }



}