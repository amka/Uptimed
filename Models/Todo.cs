using NanoidDotNet;

namespace Uptimed.Models;

public class Todo
{
    public string Id { get; set; } = Nanoid.Generate();

    public string AuthorId { get; set; }
    public ApplicationUser Author { get; set; }

    public string Title { get; set; }
    public string? Description { get; set; }
    public bool Completed { get; set; } = false;
}