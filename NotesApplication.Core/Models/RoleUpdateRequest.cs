namespace NotesApplication.Core.Models;

public class RoleUpdateRequest
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; }
}