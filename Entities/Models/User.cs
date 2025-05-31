namespace ShoesStore.Entities.Models;

using System.ComponentModel.DataAnnotations.Schema;
using Bogus;
public class User
{
    public int UserId { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string Username { get; set; }
    [Column(TypeName = "varchar(255)")]
    public required string Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? DeletedAt { get; set; } = null;
    public string? Role { get; set; } = null;

    public ICollection<Order>? Orders { get; set; } = null;







}