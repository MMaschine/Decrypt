using MockLoader;
using MockLoader.DbInfrastructure;
using MockLoader.Helpers;
using System.IO;

var connStrResult = SettingsHelper.GetConnectionString();

if (connStrResult.IsFailed)
{
    Console.WriteLine("Can't get connection string");
    Console.ReadLine();
    return;
}

var contextWrapper = new DbContextWrapper(connStrResult.Value);

var connectionResult = await contextWrapper.TryConnectionAsync();

if (connectionResult.IsFailed)
{
    Console.WriteLine($"Can't connect db with the connection string: {connectionResult}");
    Console.ReadLine();
    return;
}

var content = "";

while (true)
{
    

    Console.WriteLine("Enter the path to .js file with mock data (or type 'q')");

    var input = Console.ReadLine();

    if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
        return;

    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("Path is empty. Try again.\n");
        continue;
    }

    var filePath = input.Trim();

    try
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File does not exist. Try again.\n");
            continue;
        }

        var extension = Path.GetExtension(filePath);

        if (!string.Equals(extension, ".js", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(".js file required. Try again.\n");
            continue;
        }

        Console.WriteLine("Parsing...");

        content = await File.ReadAllTextAsync(filePath);
        break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Can't read: {ex.Message}\n");
    }
}

var extractor = new DataExtractor(contextWrapper);
await extractor.PopulateDataBaseAsync(content);

Console.WriteLine("Job completed!");
Console.ReadLine();