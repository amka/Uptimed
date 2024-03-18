namespace Uptimed.Models.Request;

public record UpdateMonitorRequest(
    string Url,
    string? RequestBody,
    string? RequestHeaders,
    string? RequestMethod,
    int? RequestTimeout,
    bool IsEnabled
);