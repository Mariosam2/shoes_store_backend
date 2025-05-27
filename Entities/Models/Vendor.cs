namespace ShoesStore.Entities.Models;


public class Vendor
{
    public int VendorId { get; set; }
    public required string VendorUuid { get; set; }
    public required string Name { get; set; }

}