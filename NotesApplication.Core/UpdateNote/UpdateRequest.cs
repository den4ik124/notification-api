namespace NotesApplication.Core.UpdateNote;

public class UpdateRequest
{
    public string NewName { get; set; } = string.Empty;
    public string NewDescription { get; set; } = string.Empty;
}