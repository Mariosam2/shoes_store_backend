using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesStore.Entities.Models;


public class Category
{
    public int CategoryId { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string CategoryUuid { get; set; }

    public required string Name { get; set; }

    public ICollection<Product>? Products { get; set; } = null;


}