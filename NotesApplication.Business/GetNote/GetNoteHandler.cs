using MediatR;
using Microsoft.EntityFrameworkCore;
using NotesApplication.Business.Extensions;
using NotesApplication.Data;

namespace NotesApplication.Business.GetNote;

public class GetNoteHandler : IRequestHandler<GetNoteQuery, NotificationResponse?>
{
    private readonly NotesDbContext _context;

    public GetNoteHandler(NotesDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationResponse?> Handle(GetNoteQuery request, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return note?.ToNoteResponse() ?? null;
    }
}

//TODO если чет меняется в базюке это КОМАНДЫ. Query - получает, не меняет 