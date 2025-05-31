using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesStore.Entities.Models;


public class Order
{
    public int OrderId { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string OrderUuid { get; set; }

    //TODO: add stripe id if needed

    public required DateTime CreatedAt { get; set; }

    public DateTime? FullfilledAt { get; set; }

    public DateTime? CanceledAt { get; set; }

    public DateTime? RefundedAt { get; set; }

    public required User User { get; set; }


}