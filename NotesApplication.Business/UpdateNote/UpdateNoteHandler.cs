using MediatR;
using Microsoft.EntityFrameworkCore;
using NotesApplication.Core;
using NotesApplication.Data;
using System.Net;

namespace NotesApplication.Business.UpdateNote;

internal class UpdateNoteHandler : IRequestHandler<UpdateNoteCommand, Result>
{
    private readonly NotesDbContext _context;

    public UpdateNoteHandler(NotesDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (note == null)
        {
            return Result.Fail("Запись с таким Id не найдена", HttpStatusCode.NotFound);
        }

        if (request.NewName == note.Name)
        {
            return Result.Fail("Введено одинаковое Имя. Нечего изменять", HttpStatusCode.BadRequest);
        }
        if (request.NewDescription == note.Description)
        {
            return Result.Fail("Введено одинаковое Описание. Нечего изменять", HttpStatusCode.BadRequest);
        }

        note.Name = request.NewName;

        if (!string.IsNullOrEmpty(request.NewDescription))
        {
            note.Description = request.NewDescription;
        }

        _context.Update(note);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}