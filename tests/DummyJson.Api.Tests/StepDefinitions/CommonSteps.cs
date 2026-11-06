using Reqnroll;
using DummyJson.Api.Tests.Support;

namespace DummyJson.Api.Tests.StepDefinitions;

[Binding]
public sealed class CommonSteps(ScenarioState state)
{
    [Then(@"the response status should be (\d+) and the content type should be JSON")]
    public void ThenTheResponseShouldBeJsonWithStatus(int status) =>
        ResponseAssertions.HasStatusAndJson(state.Response
            ?? throw new InvalidOperationException("No API request has been sent."), status);
}
