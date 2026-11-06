using DummyJson.Api.Clients;
using DummyJson.Api.Configuration;
using DummyJson.Api.Models;
using DummyJson.Api.Tests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.Api.Tests.StepDefinitions;

[Binding]
public sealed class AuthSteps(AuthClient client, ApiSettings settings, ScenarioState state)
{
    [Given("the configured DummyJSON user credentials")]
    public void GivenTheConfiguredCredentials()
    {
        Assert.That(settings.Username, Is.Not.Empty);
        Assert.That(string.IsNullOrWhiteSpace(settings.Password), Is.False, "A password must be configured.");
    }

    [When("I authenticate the user")]
    public async Task WhenIAuthenticate() =>
        state.Response = await client.LoginAsync(new LoginRequest(settings.Username, settings.Password));

    [Then("a non-empty access token should be stored for this scenario")]
    public void ThenStoreAccessToken()
    {
        var body = state.Body<LoginResponse>();
        Assert.That(string.IsNullOrWhiteSpace(body.AccessToken), Is.False, "Login must return an access token.");
        Assert.That(body.Username, Is.EqualTo(settings.Username));
        state.AccessToken = body.AccessToken;
    }

    [Given("I have authenticated as the configured DummyJSON user")]
    public async Task GivenIAmAuthenticated()
    {
        GivenTheConfiguredCredentials();
        await WhenIAuthenticate();
        ResponseAssertions.HasStatusAndJson(state.Response!, 200);
        ThenStoreAccessToken();
    }

    [When("I request the current authenticated user")]
    public async Task WhenIRequestCurrentUser() =>
        state.Response = await client.GetCurrentUserAsync(state.AccessToken
            ?? throw new InvalidOperationException("Authenticate before requesting the current user."));

    [Then(@"the current user should have id (\d+), first name ""([^""]*)"" and last name ""([^""]*)""")]
    public void ThenValidateIdentity(int id, string firstName, string lastName)
    {
        var user = state.Body<User>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(user.Id, Is.EqualTo(id));
            Assert.That(user.FirstName, Is.EqualTo(firstName));
            Assert.That(user.LastName, Is.EqualTo(lastName));
        }
    }
}
