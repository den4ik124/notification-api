using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotesApplication.Core;
using NotesApplication.Data;
using System.Net;

namespace NotesApplication.Business.DeleteNote;

public class DeleteNoteHandler : IRequestHandler<DeleteNoteCommand, Result>
{
    private readonly NotesDbContext _context;
    private readonly ILogger<DeleteNoteHandler> _logger;

    public DeleteNoteHandler(ILogger<DeleteNoteHandler> logger, NotesDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (note == null)
        {
            var message = "Запись с таким Id не найдена";
            _logger.LogWarning(message);
            return Result.Fail(message, HttpStatusCode.NotFound);
        }
        _context.Remove(note);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}