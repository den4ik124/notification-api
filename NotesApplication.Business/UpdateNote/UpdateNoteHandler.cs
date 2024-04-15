using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotesApplication.Core;
using NotesApplication.Data;
using System.Net;

namespace NotesApplication.Business.UpdateNote;

internal class UpdateNoteHandler : IRequestHandler<UpdateNoteCommand, Result>
{
    private readonly NotesDbContext _context;
    private readonly ILogger<UpdateNoteHandler> _logger;

    public UpdateNoteHandler(ILogger<UpdateNoteHandler> logger, NotesDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (note == null)
        {
            var message = "Запись с таким Id не найдена";
            _logger.LogWarning(message);
            return Result.Fail(message, HttpStatusCode.NotFound);
        }

        if (request.NewName == note.Name)
        {
            var message = "Введено одинаковое Имя. Нечего изменять";
            _logger.LogWarning(message);
            return Result.Fail(message, HttpStatusCode.BadRequest);
        }
        if (request.NewDescription == note.Description)
        {
            var message = "Введено одинаковое Описание. Нечего изменять";
            _logger.LogWarning(message);
            return Result.Fail(message, HttpStatusCode.BadRequest);
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