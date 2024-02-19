using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uptimed.Data;
using Uptimed.Models;
using Uptimed.Models.Request;

namespace Uptimed.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class TodoController(UptimedDbContext db, UserManager<ApplicationUser> userManager) : Controller
{
    // GET
    [HttpGet]
    public async Task<List<Todo>> GetTodosAsync()
    {
        var userId = userManager.GetUserId(HttpContext.User);
        return await db.Todos.Where(t => t.AuthorId == userId).ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodoAsync(CreateTodoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = userManager.GetUserId(HttpContext.User);

        var todoItem = new Todo
        {
            AuthorId = userId!,
            Title = request.Title,
            Description = request.Description,
            Completed = false,
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        return CreatedAtAction("CreateTodo", new { id = todoItem.Id }, todoItem);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoAsync(string id)
    {
        var todo = await db.Todos.Where(t => t.Id == id).FirstOrDefaultAsync();
        if (todo == null) return NotFound();

        return Ok(todo);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoAsync(string id)
    {
        var userId = userManager.GetUserId(HttpContext.User);
        var todo = await db.Todos.Where(t =>
            t.Id == id && t.AuthorId == userId).FirstOrDefaultAsync();
        if (todo == null) return NotFound();
        
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        
        return Ok();
    }
}