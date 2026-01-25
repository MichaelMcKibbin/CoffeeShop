using System;
using SQLite;

namespace CoffeeShop.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [NotNull, Unique]
    public string Username { get; set; } = "";
    
    [NotNull]
    public string PasswordHash { get; set; } = "";
    
    public string FullName { get; set; } = "";
    
    public string PhoneNumber { get; set; } = "";
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
