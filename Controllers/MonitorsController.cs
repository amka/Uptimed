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
    // GET
    [HttpGet]
    [ProducesResponseType<Monitor>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMonitorsAsync([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var total = await monitorService.GetUserMonitorsCountAsync(user);
        var monitors = await monitorService.GetUserMonitorsAsync(user, page, pageSize);
        return Ok(new GetMonitorsResponse(monitors, total));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMonitorAsync(string id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        
        var monitor = await monitorService.GetUserMonitorAsync(user, id);
        if (monitor == null) return NotFound();
        
        return Ok(monitor);
    }

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