using NotesApplication.Core;

namespace NotesApplication.Business.Extensions;

public static class DomainExtension
{
    public static NotificationResponse ToNoteResponse(this Note note)
    {
        return new NotificationResponse(note.Name, note.Description, note.Id);
    }
}