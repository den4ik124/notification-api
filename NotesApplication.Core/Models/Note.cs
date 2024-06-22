using System.ComponentModel.DataAnnotations;

namespace NotesApplication.Core.Models;

public class Note : IEquatable<Note>
{
    public Note(string name, string description)
    {
        Id = Guid.NewGuid();

        Name = name;
        Description = description;
        Time = DateTime.Now;
    }

    public Note()
    {
    }

    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Time { get; set; }

    public bool Equals(Note? other)
    {
        return GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Description);
    }
}