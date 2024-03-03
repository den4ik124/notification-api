namespace NotesApplication.Core;

public class Note : IEquatable<Note>
{
    public Note(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
    }

    public Note()
    {
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public bool Equals(Note? other)
    {
        return this == other;
    }
}