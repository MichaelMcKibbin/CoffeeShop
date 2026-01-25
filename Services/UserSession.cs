using System;
using System.Security.Cryptography;
using System.Text;
using CoffeeShop.Models;

namespace CoffeeShop.Services;

public class UserSession
{
    private readonly DatabaseService _database;
    
    public UserSession(DatabaseService database)
    {
        _database = database;
    }

    public User? CurrentUser { get; private set; }
    
    public bool IsLoggedIn => CurrentUser != null;

    public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return (false, "Username and password are required");
        }

        var user = await _database.GetUserByUsernameAsync(username);
        
        if (user == null)
        {
            return (false, "User not found");
        }

        var passwordHash = HashPassword(password);
        
        if (user.PasswordHash != passwordHash)
        {
            return (false, "Invalid password");
        }

        CurrentUser = user;
        return (true, "Login successful");
    }

    public async Task<(bool Success, string Message)> RegisterAsync(string username, string password, string fullName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return (false, "Username and password are required");
        }

        if (password.Length < 4)
        {
            return (false, "Password must be at least 4 characters");
        }

        var existingUser = await _database.GetUserByUsernameAsync(username);
        
        if (existingUser != null)
        {
            return (false, "Username already exists");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            FullName = fullName,
            PhoneNumber = phoneNumber
        };

        await _database.CreateUserAsync(user);
        
        // Automatically log in after registration
        CurrentUser = user;
        
        return (true, "Registration successful");
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    private static string HashPassword(string password)
    {
        // Simple hash for local storage - in production, use BCrypt or similar
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
