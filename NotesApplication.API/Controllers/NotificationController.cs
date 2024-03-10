using Microsoft.AspNetCore.Mvc;
using NotesApplication.Core;
using NotesApplication.Core.CreateNote;
using NotesApplication.Core.GetAllNotes;
using NotesApplication.Core.UpdateNote;
using NotesApplication.Data;

namespace NotesApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private static List<Note> _notifications = [
        new Note()
        {
            Id = new Guid("aaeaf34b-1cef-4f7c-b87d-fda12484edd8"),
            Name = "Name1",
            Description = "Description1"
        }
        ];

    private readonly NotesDbContext _context;

    public NotificationController(NotesDbContext context)
    {
        _context = context;
    }

    public virtual List<Note> Notes { get; set; }

    [HttpPut("update")]
    public async Task<ActionResult<bool>> Update(UpdateRequest request)
    {
        var note = _context.Notes.FirstOrDefault(x => x.Id == request.Id);

        if (note == null)
        {
            return NotFound();
        }

        note.Name = request.NewName;

        if (!string.IsNullOrEmpty(request.NewDescription))
        {
            note.Description = request.NewDescription;
        }
        _context.SaveChanges();
        return true;
    }

    [HttpGet("getNotes")]
    public async Task<IEnumerable<NotificationResponse>> GetAllNotes()
    {
        return _context.Notes.Select(x => new NotificationResponse(x.Name, x.Description, x.Id));
    }

    [HttpPost("create")]
    public async Task<NotificationResponse> CreateNotification(CreateRequest request)
    {
        var newNote = new Note(request.Name, request.Description);

        _context.Add(newNote);
        _context.SaveChanges();

        return new NotificationResponse(newNote.Name, newNote.Description, newNote.Id);
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        var noteToRemove = _context.Notes.FirstOrDefault(x => x.Id == id);

        if (noteToRemove == null)
        {
            return NotFound();
        }

        _context.Notes.Remove(noteToRemove);

        return _context.SaveChanges() > 0;
    }
}