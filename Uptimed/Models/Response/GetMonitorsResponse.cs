namespace Uptimed.Models.Response;

public record GetMonitorsResponse(List<Monitor> Monitors, int Total);