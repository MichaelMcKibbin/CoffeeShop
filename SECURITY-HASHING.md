# Password Hashing Implementation - Coffee Shop App

## Overview

The Coffee Shop application uses **SHA-256 (Secure Hash Algorithm 256-bit)** for password hashing to protect user credentials. This document explains the implementation, security considerations, and recommendations for production deployment.

## Table of Contents

- [What is SHA-256?](#what-is-sha-256)
- [Implementation](#implementation)
- [How It Works](#how-it-works)
- [Registration Flow](#registration-flow)
- [Login Flow](#login-flow)
- [Security Analysis](#security-analysis)
- [Limitations](#limitations)
- [Production Recommendations](#production-recommendations)
- [Code Examples](#code-examples)

---

## What is SHA-256?

**SHA-256** (Secure Hash Algorithm 256-bit) is a cryptographic hash function that:

- Produces a **256-bit (32-byte)** hash value
- Is part of the **SHA-2 family** designed by the NSA
- Is a **one-way function** - cannot be reversed
- Is **deterministic** - same input always produces the same output
- Is **fast to compute** - which is both an advantage and a security concern

### Key Characteristics

| Property | Description |
|----------|-------------|
| **Algorithm** | SHA-256 (SHA-2 family) |
| **Output Size** | 256 bits (32 bytes, 44 characters Base64) |
| **Collision Resistance** | Computationally infeasible to find two inputs with same hash |
| **Pre-image Resistance** | Cannot derive original password from hash |
| **Speed** | Very fast (~600 MB/s on modern CPUs) |

---

## Implementation

### Location
`Services/UserSession.cs` - Lines 86-93

### Code

```csharp
private static string HashPassword(string password)
{
    // Simple hash for local storage - in production, use BCrypt or similar
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}
```

### Dependencies

```csharp
using System;
using System.Security.Cryptography;  // Provides SHA256 class
using System.Text;                    // Provides Encoding
```

---

## How It Works

### Step-by-Step Process

```
Plain Password → UTF-8 Bytes → SHA-256 Hash → Base64 String → Database
```

#### 1. **Create SHA-256 Instance**
```csharp
using var sha256 = SHA256.Create();
```
- Creates a new SHA-256 hashing algorithm instance
- `using` statement ensures proper disposal of cryptographic resources

#### 2. **Convert Password to Bytes**
```csharp
var bytes = Encoding.UTF8.GetBytes(password);
```
- Converts the string password to UTF-8 byte array
- Example: `"myPass123"` → `[109, 121, 80, 97, 115, 115, 49, 50, 51]`

#### 3. **Compute Hash**
```csharp
var hash = sha256.ComputeHash(bytes);
```
- Applies SHA-256 algorithm to the byte array
- Produces a 32-byte (256-bit) hash value
- Example output: `[174, 201, 78, ..., 245]` (32 bytes)

#### 4. **Encode to Base64**
```csharp
return Convert.ToBase64String(hash);
```
- Converts byte array to Base64 string for database storage
- Example: `"rsk+3Hw...f1Q=="` (44 characters)
- Makes hash human-readable and database-friendly

---

## Registration Flow

### Process Diagram

```
User enters password
        ↓
HashPassword(password)
        ↓
SHA-256 Algorithm
        ↓
Store hash in database
        ↓
Plain password NEVER stored
```

### Code Implementation

**Location:** `Services/UserSession.cs` - Lines 46-79

```csharp
public async Task<(bool Success, string Message)> RegisterAsync(
    string username, string password, string fullName, string phoneNumber)
{
    // Validation
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        return (false, "Username and password are required");
    }

    if (password.Length < 4)
    {
        return (false, "Password must be at least 4 characters");
    }

    // Check for existing user
    var existingUser = await _database.GetUserByUsernameAsync(username);
    if (existingUser != null)
    {
        return (false, "Username already exists");
    }

    // Create user with HASHED password
    var user = new User
    {
        Username = username,
        PasswordHash = HashPassword(password),  // ← Password is hashed here
        FullName = fullName,
        PhoneNumber = phoneNumber
    };

    await _database.CreateUserAsync(user);
    CurrentUser = user;
    
    return (true, "Registration successful");
}
```

### Example

**Input:**
- Username: `mike`
- Password: `coffee123`

**Output in Database:**
- Username: `mike`
- PasswordHash: `Cxj5+Vqk3hW8fN2pQr7sT9uXy1aB4cD6eF8gH0iJ2kL4=` (example)

**Security Note:** The plain password `"coffee123"` is NEVER stored in the database.

---

## Login Flow

### Process Diagram

```
User enters password
        ↓
HashPassword(entered password)
        ↓
SHA-256 Algorithm
        ↓
Compare with stored hash
        ↓
Match? → Login Success
No Match? → Login Failed
```

### Code Implementation

**Location:** `Services/UserSession.cs` - Lines 21-44

```csharp
public async Task<(bool Success, string Message)> LoginAsync(
    string username, string password)
{
    // Validation
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        return (false, "Username and password are required");
    }

    // Retrieve user from database
    var user = await _database.GetUserByUsernameAsync(username);
    if (user == null)
    {
        return (false, "User not found");
    }

    // Hash the entered password
    var passwordHash = HashPassword(password);
    
    // Compare hashes (NOT plain passwords)
    if (user.PasswordHash != passwordHash)
    {
        return (false, "Invalid password");
    }

    // Login successful
    CurrentUser = user;
    return (true, "Login successful");
}
```

### Example Login Attempt

**Scenario 1: Correct Password**
```
Entered: "coffee123"
Hashed: "Cxj5+Vqk3hW8fN2pQr7sT9uXy1aB4cD6eF8gH0iJ2kL4="
Stored: "Cxj5+Vqk3hW8fN2pQr7sT9uXy1aB4cD6eF8gH0iJ2kL4="
Result: MATCH - Login Success
```

**Scenario 2: Incorrect Password**
```
Entered: "coffee124"  (typo)
Hashed: "9aB2cD3eF4gH5iJ6kL7mN8oP9qR0sT1uV2wX3yZ4aB5="
Stored: "Cxj5+Vqk3hW8fN2pQr7sT9uXy1aB4cD6eF8gH0iJ2kL4="
Result: NO MATCH - Login Failed
```

---

## Security Analysis

### What SHA-256 Provides

#### 1. **One-Way Function**
- **Cannot be reversed** - Impossible to get the original password from the hash
- Even with the hash, attackers cannot determine the password

#### 2. **Deterministic**
- **Same password = same hash** - Enables password verification
- Necessary for the login process to work

#### 3. **Avalanche Effect**
- **Small change = completely different hash**
```
Password: "coffee123"  → Hash: "Cxj5+Vqk3h..."
Password: "coffee124"  → Hash: "9aB2cD3eF4..." (completely different)
```

#### 4. **Fixed Output Size**
- **Always 256 bits (44 characters in Base64)**
- Regardless of password length

#### 5. **Fast Computation**
- **Quick verification** - Good user experience
- Login doesn't have noticeable delay

### What This Implementation Protects Against

| Threat | Protection Level | Explanation |
|--------|------------------|-------------|
| **Database Breach** | ⚠️ Partial | Passwords aren't stored in plain text |
| **Direct Password Viewing** | ✅ Full | Database admin can't see actual passwords |
| **SQL Injection** | ✅ Full | Hash comparison doesn't expose passwords |
| **Man-in-the-Middle** | ❌ None | Requires HTTPS (not implemented) |

---

## Limitations

### ⚠️ Security Weaknesses ⚠️

This implementation has several limitations that make it unsuitable for production:

#### 1. **No Salt**

**Problem:**
- Same password = same hash across ALL users
- Enables **rainbow table attacks** (pre-computed hash databases)

**Example:**
```
User A: password="coffee123" → hash="Cxj5+Vqk..."
User B: password="coffee123" → hash="Cxj5+Vqk..." (SAME!)
```

**Attack Scenario:**
- Attacker builds a table of common passwords and their hashes
- Searches database for matching hashes
- Instantly reveals all users with that password

#### 2. **Too Fast**

**Problem:**
- Modern GPUs can compute **billions** of SHA-256 hashes per second
- Makes **brute-force attacks** feasible

**Example Speed:**
```
GPU: NVIDIA RTX 4090
Speed: ~30 billion SHA-256 hashes/second
Time to crack 8-char password: Minutes to hours
```

#### 3. **No Work Factor (Key Stretching)**

**Problem:**
- Cannot adjust computational cost as hardware improves
- Fixed algorithm difficulty

**Comparison:**
```
SHA-256:     1 hash iteration  (fast, vulnerable)
BCrypt:      2^12 iterations   (slow, secure)
Argon2:      Configurable      (adjustable security)
```

#### 4. **Vulnerable to Dictionary Attacks**

**Problem:**
- Common passwords can be quickly identified

**Example Dictionary Attack:**
```
Common passwords list:
"123456"    → hash → compare with database
"password"  → hash → compare with database
"coffee123" → hash → FOUND IN DATABASE!
```

---

## Production Recommendations

### Use Proper Password Hashing

For production applications, replace SHA-256 with one of these algorithms:

#### 1. **BCrypt** (Recommended)

**Features:**
- Built-in salt generation
- Configurable work factor (cost)
- Industry standard
- Resistant to GPU acceleration

**Implementation Example:**

```csharp
// Install NuGet Package: BCrypt.Net-Next

using BCrypt.Net;

// Registration - Hash password
public async Task<(bool, string)> RegisterAsync(string username, string password, ...)
{
    var user = new User
    {
        Username = username,
        PasswordHash = BCrypt.HashPassword(password, workFactor: 12),  // ← Use BCrypt
        ...
    };
    await _database.CreateUserAsync(user);
    return (true, "Registration successful");
}

// Login - Verify password
public async Task<(bool, string)> LoginAsync(string username, string password)
{
    var user = await _database.GetUserByUsernameAsync(username);
    if (user == null)
        return (false, "User not found");

    // BCrypt handles salt comparison automatically
    if (!BCrypt.Verify(password, user.PasswordHash))
        return (false, "Invalid password");

    CurrentUser = user;
    return (true, "Login successful");
}
```

**BCrypt Output Example:**
```
$2a$12$R9h/cIPz0gi.URNNX3kh2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUW
 ││ ││                    │
 ││ ││                    └─ Salt (22 chars) + Hash (31 chars)
 ││ ││
 ││ │└─ Work factor (2^12 = 4,096 iterations)
 ││ └── Minor version
 │└──── Major version (2a)
 └───── Bcrypt identifier
```

#### 2. **Argon2** (Most Secure)

**Features:**
- Winner of Password Hashing Competition (2015)
- Memory-hard algorithm (resistant to GPU attacks)
- Configurable memory, time, and parallelism

**Implementation Example:**

```csharp
// Install NuGet Package: Konscious.Security.Cryptography.Argon2

using Konscious.Security.Cryptography;
using System.Security.Cryptography;

public static string HashPassword(string password)
{
    // Generate random salt
    byte[] salt = RandomNumberGenerator.GetBytes(16);
    
    using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
    {
        Salt = salt,
        DegreeOfParallelism = 8,  // CPU cores
        Iterations = 4,            // Time cost
        MemorySize = 65536        // Memory cost (64 MB)
    };

    byte[] hash = argon2.GetBytes(16);
    
    // Combine salt + hash for storage
    byte[] hashBytes = new byte[salt.Length + hash.Length];
    Array.Copy(salt, 0, hashBytes, 0, salt.Length);
    Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);
    
    return Convert.ToBase64String(hashBytes);
}
```

#### 3. **PBKDF2** (Legacy, Still Acceptable)

**Features:**
- Built into .NET (no extra packages)
- Configurable iterations
- Less secure than BCrypt/Argon2

**Implementation Example:**

```csharp
using System.Security.Cryptography;

public static string HashPassword(string password)
{
    byte[] salt = RandomNumberGenerator.GetBytes(16);
    
    using var pbkdf2 = new Rfc2898DeriveBytes(
        password, 
        salt, 
        iterations: 100000,  // OWASP recommends 600,000+ for PBKDF2-SHA256
        HashAlgorithmName.SHA256
    );
    
    byte[] hash = pbkdf2.GetBytes(32);
    
    // Combine salt + hash
    byte[] hashBytes = new byte[salt.Length + hash.Length];
    Array.Copy(salt, 0, hashBytes, 0, salt.Length);
    Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);
    
    return Convert.ToBase64String(hashBytes);
}
```

### Comparison Table

| Algorithm | Speed | GPU Resistant | Memory Hard | Recommended | Work Factor Configurable |
|-----------|-------|---------------|-------------|-------------|-------------------------|
| **SHA-256** | Very Fast | ❌ No | ❌ No | ❌ Not for passwords | ❌ No |
| **BCrypt** | Slow | ✅ Yes | ⚠️ Partial | ✅ Yes | ✅ Yes |
| **Argon2** | Configurable | ✅ Yes | ✅ Yes | ✅ Best | ✅ Yes |
| **PBKDF2** | Medium | ⚠️ Partial | ❌ No | ⚠️ Acceptable | ✅ Yes |

---

## Code Examples

### Current Implementation (Educational Only)

```csharp
// Services/UserSession.cs

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

    // Registration with SHA-256 hashing
    public async Task<(bool Success, string Message)> RegisterAsync(
        string username, string password, string fullName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Username and password are required");

        if (password.Length < 4)
            return (false, "Password must be at least 4 characters");

        var existingUser = await _database.GetUserByUsernameAsync(username);
        if (existingUser != null)
            return (false, "Username already exists");

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),  // SHA-256 hash
            FullName = fullName,
            PhoneNumber = phoneNumber
        };

        await _database.CreateUserAsync(user);
        CurrentUser = user;
        
        return (true, "Registration successful");
    }

    // Login with SHA-256 verification
    public async Task<(bool Success, string Message)> LoginAsync(
        string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Username and password are required");

        var user = await _database.GetUserByUsernameAsync(username);
        if (user == null)
            return (false, "User not found");

        var passwordHash = HashPassword(password);
        
        if (user.PasswordHash != passwordHash)
            return (false, "Invalid password");

        CurrentUser = user;
        return (true, "Login successful");
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    // SHA-256 hashing implementation
    private static string HashPassword(string password)
    {
        // ⚠️ WARNING: Simple hash for local storage only
        // In production, use BCrypt, Argon2, or PBKDF2
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
```

### Testing Hash Consistency

```csharp
// Unit test example
[Test]
public void HashPassword_SameInput_ProducesSameHash()
{
    string password = "coffee123";
    
    string hash1 = HashPassword(password);
    string hash2 = HashPassword(password);
    
    Assert.AreEqual(hash1, hash2);  // Should be identical
}

[Test]
public void HashPassword_DifferentInput_ProducesDifferentHash()
{
    string password1 = "coffee123";
    string password2 = "coffee124";
    
    string hash1 = HashPassword(password1);
    string hash2 = HashPassword(password2);
    
    Assert.AreNotEqual(hash1, hash2);  // Should be different
}
```

---

## Additional Security Recommendations

### 1. **Add HTTPS/TLS**
```csharp
// Ensure all network communication is encrypted
// Configure in Android: android:usesCleartextTraffic="false"
```

### 2. **Implement Rate Limiting**
```csharp
// Prevent brute-force attacks
private Dictionary<string, int> _loginAttempts = new();

public async Task<(bool, string)> LoginAsync(string username, string password)
{
    // Check login attempts
    if (_loginAttempts.TryGetValue(username, out int attempts) && attempts >= 5)
    {
        return (false, "Account locked. Too many failed attempts.");
    }
    
    // ... rest of login logic
    
    // On failure, increment counter
    if (!success)
    {
        _loginAttempts[username] = attempts + 1;
    }
}
```

### 3. **Add Password Strength Requirements**
```csharp
public static bool IsPasswordStrong(string password)
{
    return password.Length >= 8 &&
           password.Any(char.IsUpper) &&
           password.Any(char.IsLower) &&
           password.Any(char.IsDigit) &&
           password.Any(ch => !char.IsLetterOrDigit(ch));
}
```

### 4. **Implement Account Lockout**
```csharp
// After N failed attempts, lock account for X minutes
// Store in database: LockedUntil DateTime column
```

---

## Summary

### Current Implementation

**Appropriate for:**
- Educational projects
- Local-only applications
- Proof-of-concept demonstrations
- Learning about password security

**Not suitable for:**
- Production applications
- Internet-facing services
- Applications handling sensitive data
- Commercial products

### Key Takeaways

1. **SHA-256 is a cryptographic hash**, not a password hashing algorithm
2. **Current implementation prevents plain-text storage** (good for learning)
3. **Production requires salted, slow hashing** (BCrypt, Argon2, PBKDF2)
4. **Security is a process**, not a one-time implementation

### Migration Path to Production

```
Current: SHA-256 (no salt, fast)
         ↓
Step 1:  Add PBKDF2 with salt and iterations
         ↓
Step 2:  Upgrade to BCrypt (recommended)
         ↓
Step 3:  Consider Argon2 (maximum security)
```

---

## References

### Standards & Guidelines

- **OWASP Password Storage Cheat Sheet**  
  https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html

- **NIST Digital Identity Guidelines (SP 800-63B)**  
  https://pages.nist.gov/800-63-3/sp800-63b.html

- **Password Hashing Competition (PHC)**  
  https://www.password-hashing.net/

### Algorithms

- **SHA-256 Specification (FIPS 180-4)**  
  https://csrc.nist.gov/publications/detail/fips/180/4/final

- **BCrypt Algorithm**  
  https://en.wikipedia.org/wiki/Bcrypt

- **Argon2 Specification**  
  https://github.com/P-H-C/phc-winner-argon2

- **PBKDF2 (RFC 2898)**  
  https://tools.ietf.org/html/rfc2898

### .NET Resources

- **.NET Cryptography API**  
  https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography

- **BCrypt.Net-Next NuGet Package**  
  https://www.nuget.org/packages/BCrypt.Net-Next/

- **Konscious.Security.Cryptography.Argon2 NuGet Package**  
  https://www.nuget.org/packages/Konscious.Security.Cryptography.Argon2/

---

## Document Information

**Created:** January 2025  
**Project:** Coffee Shop - .NET MAUI Application  
**Developer:** Michael McKibbin (ATU Student# L00197067)  
**Version:** 1.0  
**Related Files:** `Services/UserSession.cs`, `README.md`

---

*This document is part of the Coffee Shop application's technical documentation. For general project information, see the main [README.md](README.md) file.*
