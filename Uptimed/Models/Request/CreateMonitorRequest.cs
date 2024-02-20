namespace Uptimed.Models.Request;

public record CreateMonitorRequest(
    string Url,
    string? RequestBody,
    string? RequestHeaders,
    string? RequestMethod,
    int?  RequestTimeout
);