using MediatR;
using Microsoft.EntityFrameworkCore;
using NotesApplication.Core;
using NotesApplication.Data;
using System.Net;

namespace NotesApplication.Business.DeleteNote;

public class DeleteNoteHandler : IRequestHandler<DeleteNoteCommand, Result>
{
    private readonly NotesDbContext _context;

    public DeleteNoteHandler(NotesDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (note == null)
        {
            return Result.Fail("Запись с таким Id не найдена", HttpStatusCode.NotFound);
        }
        _context.Remove(note);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}