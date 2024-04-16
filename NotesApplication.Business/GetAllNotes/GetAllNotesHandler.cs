using MediatR;
using Microsoft.EntityFrameworkCore;
using NotesApplication.Business.Extensions;
using NotesApplication.Data;

namespace NotesApplication.Business.GetAllNotes;

public class GetAllNotesHandler : IRequestHandler<GetAllNotesQuery, IEnumerable<NotificationResponse>>
{
    private readonly NotesDbContext _context;

    public GetAllNotesHandler(NotesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationResponse>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Notes.Select(x => x.ToNoteResponse()).ToListAsync(cancellationToken);
    }
}