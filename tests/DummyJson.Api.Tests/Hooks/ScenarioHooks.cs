using DummyJson.Api.Clients;
using DummyJson.Api.Configuration;
using DummyJson.Api.Http;
using Reqnroll;
using Reqnroll.BoDi;

namespace DummyJson.Api.Tests.Hooks;

[Binding]
public sealed class ScenarioHooks(IObjectContainer container, IReqnrollOutputHelper output)
{
    private ApiTransport? _transport;

    [BeforeScenario(Order = 0)]
    public void RegisterScenarioDependencies()
    {
        var settings = ApiSettings.Load(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
        _transport = new ApiTransport(settings, output.WriteLine);
        container.RegisterInstanceAs(settings);
        container.RegisterInstanceAs(new AuthClient(_transport));
        container.RegisterInstanceAs(new UsersClient(_transport));
        container.RegisterInstanceAs(new ProductsClient(_transport));
        container.RegisterInstanceAs(new CartsClient(_transport));
    }

    [AfterScenario]
    public void DisposeScenarioClient() => _transport?.Dispose();
}
