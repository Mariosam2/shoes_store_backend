namespace ShoesStore.Entities.Models;



public class Product
{
    public int ProductId { get; set; }
    public required string ProductUuid { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public ICollection<Media>? Medias { get; set; } = null;

    public required Vendor Vendor { get; set; }



}