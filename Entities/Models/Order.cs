using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
    public required string CustomerName { get; set; }
    public required string Address { get; set; }
    public required string CustomerEmail { get; set; }

    [Precision(5, 2)]
    public required decimal Amount { get; set; }
    public required string ChargeID { get; set; }
    public required string PaymentIntentID { get; set; }
    public required string PaymentMethodID { get; set; }


}