namespace ShoesStore.Entities.Models;


using Bogus;
public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }

    public required string Password { get; set; }

    public required DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; } = null;
    public string? Role { get; set; } = null;







}