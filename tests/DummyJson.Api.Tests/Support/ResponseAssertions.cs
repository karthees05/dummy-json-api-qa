using DummyJson.Api.Http;
using NUnit.Framework;

namespace DummyJson.Api.Tests.Support;

public static class ResponseAssertions
{
    public static void HasStatusAndJson(IApiResponse response, int status)
    {
        Assert.That((int)response.StatusCode, Is.EqualTo(status), "HTTP status code");
        Assert.That(response.ContentType, Does.StartWith("application/json"), "Response Content-Type");
    }
}
