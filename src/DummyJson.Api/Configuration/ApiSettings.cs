using System.Text.Json;

namespace DummyJson.Api.Configuration;

public sealed record ApiSettings
{
    public string BaseUrl { get; init; } = "https://dummyjson.com";
    public int TimeoutSeconds { get; init; } = 30;
    public string Username { get; init; } = "emilys";
    public string Password { get; init; } = "emilyspass";

    public static ApiSettings Load(string path)
    {
        var settings = JsonSerializer.Deserialize<ApiSettings>(File.ReadAllText(path),
            new JsonSerializerOptions(JsonSerializerDefaults.Web))
            ?? throw new InvalidOperationException("API settings must be a JSON object.");

        settings = settings with
        {
            BaseUrl = Environment.GetEnvironmentVariable("DUMMYJSON_BASE_URL") ?? settings.BaseUrl,
            Username = Environment.GetEnvironmentVariable("DUMMYJSON_USERNAME") ?? settings.Username,
            Password = Environment.GetEnvironmentVariable("DUMMYJSON_PASSWORD") ?? settings.Password
        };

        if (!Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
            throw new InvalidOperationException("BaseUrl must be an absolute HTTP(S) URL.");
        if (settings.TimeoutSeconds <= 0)
            throw new InvalidOperationException("TimeoutSeconds must be positive.");
        if (string.IsNullOrWhiteSpace(settings.Username) || string.IsNullOrWhiteSpace(settings.Password))
            throw new InvalidOperationException("DummyJSON username and password must be supplied.");

        return settings;
    }
}
