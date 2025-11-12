using System;
using System.Security.Cryptography;
using System.Text;

public class Program
{
    public static void Main()
    {
        string password = "Admin123";
        string hashedPassword = HashPassword(password);
        
        Console.WriteLine($"Original Password: {password}");
        Console.WriteLine($"Hashed Password: {hashedPassword}");
        Console.WriteLine();
        Console.WriteLine("SQL Query to insert admin:");
        Console.WriteLine($"UPDATE Users SET PasswordHash = '{hashedPassword}' WHERE Email = 'sujithkumar.kanini@outlook.com';");
    }
    
    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}


O2Esdae1BIpDX7bsgeUv+S1teVqLWpwXBw9qY8l6U7I=