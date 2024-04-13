using MediatR;
using NotesApplication.Business.Behavior;
using NotesApplication.Core;

namespace NotesApplication.Business.DeleteNote;

public record DeleteNoteCommand(Guid Id) : IRequest<Result>, ITransactional;