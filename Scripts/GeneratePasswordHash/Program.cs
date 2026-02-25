using System;

var password = args.Length > 0 ? args[0] : "Admin123!";
Console.WriteLine($"{password} için BCrypt hash:");
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11));
