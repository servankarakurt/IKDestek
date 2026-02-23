using System;
using BCrypt.Net;

// Test BCrypt hash verification
string password = "Admin123!";
string hash = "$2a$11$S/kP4U1vXw.YQhA.T2JvQ.B0d4aA3n3m3bJ4b8g5a5dG1E4h7iY9O";

try
{
    bool isValid = BCrypt.Verify(password, hash);
    Console.WriteLine($"Password verification result: {isValid}");
    
    // Generate a new hash for comparison
    string newHash = BCrypt.HashPassword(password);
    Console.WriteLine($"New hash for '{password}': {newHash}");
    
    // Verify the new hash
    bool newHashValid = BCrypt.Verify(password, newHash);
    Console.WriteLine($"New hash verification: {newHashValid}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
