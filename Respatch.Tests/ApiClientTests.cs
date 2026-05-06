using System.Net;
using Respatch.Services;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Respatch.Tests;

public class ApiClientTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly ApiClient _client;
    private const string ValidToken = "valid-token";

    public ApiClientTests()
    {
        _server = WireMockServer.Start();
        _client = new ApiClient(new HttpClient());
    }

    public void Dispose() => _server.Stop();

    private string BaseUrl => _server.Url!;

    [Fact]
    public async Task VerifyProjectAsync_ValidToken_DoesNotThrow()
    {
        _server.Given(Request.Create().WithPath("/_respatch/api/status").UsingGet()
                .WithHeader("X-Respatch-Token", ValidToken))
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{}"));

        await _client.VerifyProjectAsync(BaseUrl, ValidToken);
    }

    [Fact]
    public async Task VerifyProjectAsync_InvalidToken_ThrowsUnauthorized()
    {
        _server.Given(Request.Create().WithPath("/_respatch/api/status").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(401));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _client.VerifyProjectAsync(BaseUrl, "bad-token"));
    }

    [Fact]
    public async Task VerifyProjectAsync_ServerError_ThrowsHttpRequestException()
    {
        _server.Given(Request.Create().WithPath("/_respatch/api/status").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(500).WithBody("Internal Server Error"));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => _client.VerifyProjectAsync(BaseUrl, ValidToken));
    }

    [Fact]
    public async Task VerifyProjectAsync_UrlWithTrailingSlash_DoesNotThrow()
    {
        _server.Given(Request.Create().WithPath("/_respatch/api/status").UsingGet()
                .WithHeader("X-Respatch-Token", ValidToken))
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{}"));

        // URL with trailing slash must be normalized
        await _client.VerifyProjectAsync(BaseUrl + "/", ValidToken);
    }

    [Fact]
    public async Task GetWorkersAsync_ReturnsWorkers()
    {
        var json = """{"Workers":[{"Id":"w1","Transport":"async","Status":"busy","MemoryMb":32.5}]}""";
        _server.Given(Request.Create().WithPath("/_respatch/api/workers").UsingGet()
                .WithHeader("X-Respatch-Token", ValidToken))
            .RespondWith(Response.Create().WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        var result = await _client.GetWorkersAsync(BaseUrl, ValidToken);

        Assert.Single(result.Workers);
        Assert.Equal("w1", result.Workers[0].Id);
        Assert.Equal("busy", result.Workers[0].Status);
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsMessages()
    {
        var json = """{"Messages":[{"Id":"m1","MessageClass":"App\\Message\\TestMessage","Transport":"async","HandledAt":"2024-01-01T10:00:00Z","Failed":false,"DurationMs":12.3}]}""";
        _server.Given(Request.Create().WithPath("/_respatch/api/history").UsingGet()
                .WithHeader("X-Respatch-Token", ValidToken))
            .RespondWith(Response.Create().WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        var result = await _client.GetHistoryAsync(BaseUrl, ValidToken);

        Assert.Single(result.Messages);
        Assert.Equal("m1", result.Messages[0].Id);
        Assert.False(result.Messages[0].Failed);
    }
}
