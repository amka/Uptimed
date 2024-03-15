using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uptimed.Models;
using Uptimed.Models.Request;
using Uptimed.Models.Response;
using Uptimed.Services;
using Monitor = Uptimed.Models.Monitor;

namespace Uptimed.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MonitorsController(
    UserManager<ApplicationUser> userManager,
    MonitorService monitorService,
    ILogger<MonitorsController> logger) : Controller
{
    /// <summary>
    /// Controller for managing monitors.
    /// </summary>
    /// <param name="userManager">The user manager used for authorization.</param>
    /// <param name="monitorService">The service used to manage monitors.</param>
    /// <param name="logger">The logger used for logging.</param>
    /// <response code="401">User is not authorized.</response>
    /// <response code="400">User is not in the database.</response>
    /// <response code="200">Returns a list of monitors and total count.</response>
    [HttpGet]
    public async Task<IActionResult> GetMonitorsAsync([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var total = await monitorService.GetUserMonitorsCountAsync(user);
        var monitors = await monitorService.GetUserMonitorsAsync(user, page, pageSize);
        return Ok(new GetMonitorsResponse(monitors, total));
    }

    /// <summary>
    /// Retrieves a specific monitor by its id.
    /// </summary>
    /// <param name="id">The id of the monitor to retrieve.</param>
    /// <response code="401">User is not authorized.</response>
    /// <response code="404">Monitor with given id does not exist.</response>
    /// <response code="200">Returns the monitor.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMonitorAsync(string id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var monitor = await monitorService.GetUserMonitorAsync(user, id);
        if (monitor == null) return NotFound();

        return Ok(monitor);
    }

    /// <summary>
    /// Creates a new monitor.
    /// </summary>
    /// <param name="request">The request containing the monitor information.</param>
    /// <response code="401">User is not authorized.</response>
    /// <response code="201">Returns the created monitor.</response>
    /// <response code="400">Request is invalid.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<Monitor>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMonitorAsync(CreateMonitorRequest request)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var monitor = new Monitor
        {
            Url = request.Url,
            Alias = new Uri(request.Url).Host,
            Owner = user,
            RequestBody = request.RequestBody ?? "",
            RequestMethod = request.RequestMethod ?? "GET",
            RequestTimeout = request.RequestTimeout ?? 30,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await monitorService.CreateMonitorAsync(monitor);
        return Created("CreateMonitorAsync", monitor);
    }
}