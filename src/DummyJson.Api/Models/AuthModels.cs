namespace DummyJson.Api.Models;

public sealed record LoginRequest(string Username, string Password, int ExpiresInMins = 30);

public sealed record LoginResponse(
    int Id, string Username, string FirstName, string LastName,
    string AccessToken, string RefreshToken);
