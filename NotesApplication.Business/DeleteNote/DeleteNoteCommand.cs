using MediatR;
using NotesApplication.Core;

namespace NotesApplication.Business.DeleteNote;

public record DeleteNoteCommand(Guid Id) : IRequest<Result>;