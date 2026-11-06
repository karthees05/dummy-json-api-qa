namespace DummyJson.Api.Models;

public sealed record Hair(string Color, string Type);
public sealed record Coordinates(decimal Lat, decimal Lng);
public sealed record Address(
    string AddressLine, string City, string State, string StateCode,
    string PostalCode, Coordinates Coordinates, string Country)
{
    [System.Text.Json.Serialization.JsonPropertyName("address")]
    public string AddressLine { get; init; } = AddressLine;
}

public sealed record CreateUserRequest(
    string FirstName, string LastName, int Age, string Gender, Hair Hair, Address Address);

public sealed record User(
    int Id, string FirstName, string LastName, int Age, string Gender, Hair Hair, Address Address);

public sealed record UsersResponse(List<User> Users, int Total, int Skip, int Limit);
