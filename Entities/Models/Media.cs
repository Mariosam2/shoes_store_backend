namespace ShoesStore.Entities.Models;


public class Media
{
    public int MediaId { get; set; }
    public required string Path { get; set; }

    public required int ProductId { get; set; }
    public required Product Product { get; set; }
}