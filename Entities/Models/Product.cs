using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShoesStore.Entities.Models;



public class Product
{
    public int ProductId { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string StripeProductId { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string StripePriceId { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string ProductUuid { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string Title { get; set; }
    public required string Description { get; set; }

    [Precision(5, 2)]
    public required decimal Price { get; set; }

    public ICollection<Media>? Medias { get; set; } = null;

    public required Vendor Vendor { get; set; }



}