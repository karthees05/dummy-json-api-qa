using System.Globalization;
using DummyJson.Api.Clients;
using DummyJson.Api.Models;
using DummyJson.Api.Tests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.Api.Tests.StepDefinitions;

[Binding]
public sealed class UsersSteps(UsersClient client, ScenarioState state)
{
    private CreateUserRequest? _newUser;

    [When(@"I filter all users by hair colour ""([^""]*)""")]
    public async Task WhenIFilterUsers(string color) =>
        state.Response = await client.FilterByHairColorAsync(color);

    [Then(@"exactly (\d+) users should have hair colour ""([^""]*)""")]
    public void ThenValidateRedHairCount(int expected, string color)
    {
        var result = state.Body<UsersResponse>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Total, Is.EqualTo(expected), "Total filtered users");
            Assert.That(result.Users, Has.Count.EqualTo(expected), "All results must be fetched (limit=0)");
            Assert.That(result.Users.All(user => user.Hair.Color == color), Is.True, "Every user must match the filter");
            Assert.That(result.Users.Select(user => user.Id), Is.Unique);
            Assert.That(result.Skip, Is.Zero);
        }
    }

    [Then(@"the filtered users should match an independent count of all users with hair colour ""([^""]*)""")]
    public async Task ThenCompareWithFullDataset(string color)
    {
        var filtered = state.Body<UsersResponse>();
        var all = await client.GetAllAsync();
        ResponseAssertions.HasStatusAndJson(all, 200);
        var dataset = all.Body;
        Assert.That(dataset.Users, Has.Count.EqualTo(dataset.Total), "Fetch the entire user dataset");
        var expectedIds = dataset.Users.Where(user => user.Hair.Color == color).Select(user => user.Id);
        Assert.That(filtered.Users.Select(user => user.Id), Is.EquivalentTo(expectedIds));
    }

    [Given("a new user with the following details")]
    public void GivenNewUser(Table table)
    {
        var values = table.Rows.ToDictionary(row => row["Field"], row => row["Value"]);
        _newUser = new CreateUserRequest(values["FirstName"], values["LastName"],
            int.Parse(values["Age"], CultureInfo.InvariantCulture), values["Gender"],
            new Hair(values["HairColor"], values["HairType"]),
            new Address(values["Street"], values["City"], values["State"], values["StateCode"],
                values["PostalCode"], new Coordinates(
                    decimal.Parse(values["Latitude"], CultureInfo.InvariantCulture),
                    decimal.Parse(values["Longitude"], CultureInfo.InvariantCulture)), values["Country"]));
    }

    [When("I add the new user")]
    public async Task WhenIAddUser() => state.Response = await client.AddAsync(_newUser
        ?? throw new InvalidOperationException("Supply new user details first."));

    [Then("the created user should have a positive id and all the submitted details")]
    public void ThenValidateCreatedUser()
    {
        var user = state.Body<User>();
        var expected = _newUser!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(user.Id, Is.GreaterThan(0));
            Assert.That(user.FirstName, Is.EqualTo(expected.FirstName));
            Assert.That(user.LastName, Is.EqualTo(expected.LastName));
            Assert.That(user.Age, Is.EqualTo(expected.Age));
            Assert.That(user.Gender, Is.EqualTo(expected.Gender));
            Assert.That(user.Hair, Is.EqualTo(expected.Hair), "Hair colour and type");
            Assert.That(user.Address, Is.EqualTo(expected.Address), "Complete address including coordinates");
        }
    }
}
