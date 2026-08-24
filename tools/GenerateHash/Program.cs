using BCrypt.Net;

var password = "Admin@123";
var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

Console.WriteLine("Password : " + password);
Console.WriteLine("BCrypt Hash:");
Console.WriteLine(hash);
Console.WriteLine();
Console.WriteLine("-- SQL UPDATE:");
Console.WriteLine($"UPDATE Users SET PasswordHash = '{hash}' WHERE Username = 'admin';");
