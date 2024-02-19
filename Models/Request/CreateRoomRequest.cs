namespace Uptimed.Models.Request;

public class CreateRoomRequest
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
}