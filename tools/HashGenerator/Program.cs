if (args.Length == 0)
{
    Console.Error.WriteLine("Uso: dotnet run <contraseña>");
    Console.Error.WriteLine("Ejemplo: dotnet run \"MiContrasena123!\"");
    return 1;
}

var hash = BCrypt.Net.BCrypt.HashPassword(args[0], workFactor: 11);
Console.WriteLine(hash);
return 0;
