using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        string adminPassword = BCrypt.Net.BCrypt.HashPassword("Admin123!", 11);
        string voterPassword = BCrypt.Net.BCrypt.HashPassword("Voter123!", 11);

        Console.WriteLine($"Admin: {adminPassword}");
        Console.WriteLine($"Voter: {voterPassword}");
    }
}