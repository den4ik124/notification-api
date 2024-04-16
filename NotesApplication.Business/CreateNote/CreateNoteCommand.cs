using MediatR;
using NotesApplication.Business.Behavior;

namespace NotesApplication.Business.CreateNote;

public class CreateNoteCommand : IRequest, ITransactional
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}