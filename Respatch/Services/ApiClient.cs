using System.Net;
using System.Net.Http.Json;

namespace Respatch.Services;

/// <summary>
/// HTTP client for the Respatch server API. Replaces ApiClient.ts + libs/fetch.ts.
/// Sends X-Respatch-Token header on every request. Strips trailing slashes from URLs.
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    private static string NormalizeUrl(string url) => url.TrimEnd('/');

    private HttpRequestMessage BuildRequest(HttpMethod method, string url, string token, string path)
    {
        var request = new HttpRequestMessage(method, $"{NormalizeUrl(url)}/_respatch/api/{path}");
        request.Headers.Add("X-Respatch-Token", token);
        return request;
    }

    private async Task<T> GetAsync<T>(string url, string token, string path, CancellationToken ct)
    {
        using var request = BuildRequest(HttpMethod.Get, url, token, path);
        using var response = await _http.SendAsync(request, ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>(ct)
               ?? throw new InvalidOperationException($"Empty response from {path}");
    }

    private async Task PostAsync(string url, string token, string path, CancellationToken ct)
    {
        using var request = BuildRequest(HttpMethod.Post, url, token, path);
        using var response = await _http.SendAsync(request, ct);
        await EnsureSuccessAsync(response);
    }

    private async Task DeleteAsync(string url, string token, string path, CancellationToken ct)
    {
        using var request = BuildRequest(HttpMethod.Delete, url, token, path);
        using var response = await _http.SendAsync(request, ct);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Server returned {(int)response.StatusCode} {response.ReasonPhrase}: {body}",
                null,
                response.StatusCode);
        }
    }

    public async Task VerifyProjectAsync(string url, string token, CancellationToken ct = default)
    {
        using var request = BuildRequest(HttpMethod.Get, url, token, "status");
        using var response = await _http.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Invalid API token.");
        await EnsureSuccessAsync(response);
    }

    public Task<DashboardResponse> GetDashboardAsync(string url, string token, CancellationToken ct = default)
        => GetAsync<DashboardResponse>(url, token, "dashboard", ct);

    public Task<StatisticsResponse> GetStatisticsAsync(string url, string token, CancellationToken ct = default)
        => GetAsync<StatisticsResponse>(url, token, "statistics", ct);

    public Task<HistoryResponse> GetHistoryAsync(string url, string token, CancellationToken ct = default)
        => GetAsync<HistoryResponse>(url, token, "history", ct);

    public Task<MessageDetailResponse> GetMessageDetailAsync(string url, string token, string id, CancellationToken ct = default)
        => GetAsync<MessageDetailResponse>(url, token, $"history/{Uri.EscapeDataString(id)}", ct);

    public Task<TransportsResponse> GetTransportsAsync(string url, string token, CancellationToken ct = default)
        => GetAsync<TransportsResponse>(url, token, "transports", ct);

    public Task RemoveTransportMessageAsync(string url, string token, string transport, string id, CancellationToken ct = default)
        => DeleteAsync(url, token, $"transport/{Uri.EscapeDataString(transport)}/{Uri.EscapeDataString(id)}/remove", ct);

    public Task RetryFailedMessageAsync(string url, string token, string transport, string id, CancellationToken ct = default)
        => PostAsync(url, token, $"transport/{Uri.EscapeDataString(transport)}/{Uri.EscapeDataString(id)}/retry", ct);

    public Task<SchedulesResponse> GetSchedulesAsync(string url, string token, string name, CancellationToken ct = default)
        => GetAsync<SchedulesResponse>(url, token, $"schedule/{Uri.EscapeDataString(name)}", ct);

    public Task TriggerScheduleTaskAsync(string url, string token, string name, string id, string transport, CancellationToken ct = default)
        => PostAsync(url, token, $"schedules/{Uri.EscapeDataString(name)}/trigger/{Uri.EscapeDataString(id)}/{Uri.EscapeDataString(transport)}", ct);

    public Task<WorkersResponse> GetWorkersAsync(string url, string token, CancellationToken ct = default)
        => GetAsync<WorkersResponse>(url, token, "workers", ct);
}
