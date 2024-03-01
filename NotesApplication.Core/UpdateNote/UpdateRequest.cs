namespace NotesApplication.Core.UpdateNote;

public class UpdateRequest
{
    public Guid Id { get; set; }
    public string NewName { get; set; }
    public string NewDescription { get; set; }
}