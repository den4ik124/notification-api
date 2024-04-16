using MediatR;

namespace NotesApplication.Business.GetAllNotes;

public record GetAllNotesQuery() : IRequest<IEnumerable<NotificationResponse>>;