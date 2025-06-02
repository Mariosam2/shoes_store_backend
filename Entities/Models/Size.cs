using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesStore.Entities.Models;


public class Size
{
    public int SizeId { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string SizeUuid { get; set; }

    public required float SizeNumber { get; set; }

    public ICollection<Product>? Products { get; set; } = null;


}