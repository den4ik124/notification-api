namespace NotesApplication.Business;

public class NotificationResponse
{
    public NotificationResponse(string name, string description, Guid id)
    {
        Name = name;
        Description = description;
        Id = id;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}