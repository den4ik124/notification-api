using MediatR;

namespace NotesApplication.Business.GetNote;

public record GetNoteQuery(Guid Id) : IRequest<NotificationResponse>;