using MediatR;
using NotesApplication.Core;

namespace NotesApplication.Business.UpdateNote;

public class UpdateNoteCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string NewName { get; set; } = string.Empty;
    public string NewDescription { get; set; } = string.Empty;
}