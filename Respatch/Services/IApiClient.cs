namespace Respatch.Services;

public interface IApiClient
{
    Task VerifyProjectAsync(string url, string token, CancellationToken ct = default);
    Task<DashboardResponse> GetDashboardAsync(string url, string token, CancellationToken ct = default);
    Task<StatisticsResponse> GetStatisticsAsync(string url, string token, CancellationToken ct = default);
    Task<HistoryResponse> GetHistoryAsync(string url, string token, CancellationToken ct = default);
    Task<MessageDetailResponse> GetMessageDetailAsync(string url, string token, string id, CancellationToken ct = default);
    Task<TransportsResponse> GetTransportsAsync(string url, string token, CancellationToken ct = default);
    Task RemoveTransportMessageAsync(string url, string token, string transport, string id, CancellationToken ct = default);
    Task RetryFailedMessageAsync(string url, string token, string transport, string id, CancellationToken ct = default);
    Task<SchedulesResponse> GetSchedulesAsync(string url, string token, string name, CancellationToken ct = default);
    Task TriggerScheduleTaskAsync(string url, string token, string name, string id, string transport, CancellationToken ct = default);
    Task<WorkersResponse> GetWorkersAsync(string url, string token, CancellationToken ct = default);
}

// ── Response DTOs ──────────────────────────────────────────────────────────────

public record DashboardResponse(
    IReadOnlyList<TransportInfo> Transports,
    int TotalFailed,
    int TotalProcessed
);

public record TransportInfo(
    string Name,
    int MessageCount,
    int BusyWorkers,
    int IdleWorkers,
    double MemoryMb,
    double LoadFraction
);

public record StatisticsResponse(
    int Processed,
    int Failed,
    int Retried
);

public record HistoryResponse(
    IReadOnlyList<ProcessedMessageSummary> Messages
);

public record ProcessedMessageSummary(
    string Id,
    string MessageClass,
    string Transport,
    string HandledAt,
    bool Failed,
    double DurationMs
);

public record MessageDetailResponse(
    string Id,
    string MessageClass,
    string Transport,
    string HandledAt,
    bool Failed,
    double DurationMs,
    string? ErrorMessage,
    string? Payload
);

public record TransportsResponse(
    IReadOnlyList<TransportInfo> Transports
);

public record SchedulesResponse(
    IReadOnlyList<ScheduleInfo> Schedules
);

public record ScheduleInfo(
    string Id,
    string Name,
    string Transport,
    string? NextRunAt
);

public record WorkersResponse(
    IReadOnlyList<WorkerInfo> Workers
);

public record WorkerInfo(
    string Id,
    string Transport,
    string Status,
    double MemoryMb
);
