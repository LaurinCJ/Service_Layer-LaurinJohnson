using System.Net;
using System.Text.Json;

string baseUrl = args.Length > 0 ? args[0] : "http://localhost:5105/";
if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var address)
    || (address.Scheme != "http" && address.Scheme != "https"))
{
    Console.Error.WriteLine("Pass an absolute HTTP or HTTPS API base URL.");
    return;
}
using var client = new HttpClient { BaseAddress = address, Timeout = TimeSpan.FromSeconds(45) };
Console.WriteLine($"API: {client.BaseAddress}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. List games    2. Get games record by ID");
    Console.WriteLine("3. List franchises    4. Get franchises record by ID");
    Console.WriteLine("5. List platforms    6. Get platforms record by ID");
    Console.WriteLine("0. Exit");
    Console.Write("Choice: ");
    string? choice = Console.ReadLine();
    if (choice is null or "0") break;
    string? path;
    switch (choice)
    {
        case "1": path = "api/games"; break;
        case "2": path = ReadIdPath("games"); break;
        case "3": path = "api/franchises"; break;
        case "4": path = ReadIdPath("franchises"); break;
        case "5": path = "api/platforms"; break;
        case "6": path = ReadIdPath("platforms"); break;
        default: Console.WriteLine("Choose a listed option."); continue;
    }
    if (path is null) continue;
    try
    {
        using var response = await client.GetAsync(path);
        Console.WriteLine($"GET {path} -> {(int)response.StatusCode} {response.ReasonPhrase}");
        string body = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.NotFound)
            Console.WriteLine("The endpoint is not implemented yet, or this record does not exist. See PROJECT_TASKS.txt.");
        if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                Console.WriteLine(JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (JsonException) { Console.WriteLine(body); }
        }
        else if (!string.IsNullOrWhiteSpace(body)) Console.WriteLine(body);
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Cannot reach API: {ex.Message}. Start the API and check the base URL.");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("The request timed out. Check the API and, for cloud calls, the function host.");
    }
}

static string? ReadIdPath(string resource)
{
    Console.Write("ID (use one from the list response): ");
    if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
        return $"api/{resource}/{id}";
    Console.WriteLine("Enter a positive integer ID.");
    return null;
}
