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

    [HttpPut("update/{id}")]
    public async Task<ActionResult<bool>> Update(Guid id, UpdateRequest request)
    {
        var note = _context.Notes.FirstOrDefault(x => x.Id == id);
        //TODO пустой гуид из 00000
        //TODO не менять на одинаковое
        //TODO проверка даных в реквесте
        if (note == null)
        {
            return NotFound("Не найден");
        }

        note.Name = request.NewName;

        if (!string.IsNullOrEmpty(request.NewDescription))
        {
            note.Description = request.NewDescription;
        }

        return await _context.SaveChangesAsync() > 0;
    }

    [HttpGet()]
    public async Task<IEnumerable<NotificationResponse>> GetAllNotes()
    {
        return _context.Notes.Select(x => new NotificationResponse(x.Name, x.Description, x.Id));
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNotification(CreateRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Херовое имя пользователя");
        }
        else if (string.IsNullOrEmpty(request.Description))
        {
            return BadRequest("Херовое описание");
        }

        var newNote = new Note(request.Name, request.Description);

        _context.Add(newNote);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        var noteToRemove = _context.Notes.FirstOrDefault(x => x.Id == id);
        //TODO пустой гуид из 00000000-0000-0000-0000-000000000000 - guid.empty
        if (noteToRemove == null)
        {
            return NotFound();
        }

        _context.Notes.Remove(noteToRemove);

        return await _context.SaveChangesAsync() > 0;
    }
}