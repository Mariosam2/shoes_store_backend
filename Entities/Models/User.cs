namespace ShoesStore.Entities.Models;


using Bogus;
public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }

    public required string Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? DeletedAt { get; set; } = null;
    public string? Role { get; set; } = null;







}