namespace Uptimed.Models.Request;

public class CreateTodoRequest
{
    public string Title { get; set; }
    public string? Description { get; set; }
}