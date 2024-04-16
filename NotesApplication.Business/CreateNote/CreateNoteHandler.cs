using MediatR;
using NotesApplication.Core;
using NotesApplication.Data;

namespace NotesApplication.Business.CreateNote;

public class CreateNoteHandler : IRequestHandler<CreateNoteCommand>
{
    private readonly NotesDbContext _context;

    public CreateNoteHandler(NotesDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var newNote = new Note(request.Name, request.Description);

        _context.Add(newNote);
        await _context.SaveChangesAsync(cancellationToken);
    }
}