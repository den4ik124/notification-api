using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotesApplication.Business;
using NotesApplication.Business.CreateNote;
using NotesApplication.Business.DeleteNote;
using NotesApplication.Business.GetAllNotes;
using NotesApplication.Business.GetNote;
using NotesApplication.Business.UpdateNote;
using NotesApplication.Core;
using System.Net;

namespace NotesApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateNoteRequest request)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Пустой Guid");
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Херовое имя пользователя");
        }
        else if (string.IsNullOrEmpty(request.Description))
        {
            return BadRequest("Херовое описание");
        }

        var result = await _mediator.Send(new UpdateNoteCommand
        {
            Id = id,
            NewDescription = request.Description,
            NewName = request.Name
        });
        return HandleResult(result);
    }

    private IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        switch (result.StatusCode)
        {
            case HttpStatusCode.NotFound:

                return NotFound(result.Message);

            case HttpStatusCode.BadRequest:

                return BadRequest(result.Message);

            default:
                return BadRequest("плохо");
        };
    }

    [HttpGet()]
    public async Task<IEnumerable<NotificationResponse>> GetAllNotes()
    {
        return await _mediator.Send(new GetAllNotesQuery());
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNotification(CreateNoteCommand request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Херовое имя пользователя");
        }
        else if (string.IsNullOrEmpty(request.Description))
        {
            return BadRequest("Херовое описание");
        }

        await _mediator.Send(request);
        return Ok();// StatusCode(201);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Пустой Guid");
        }

        var result = await _mediator.Send(new DeleteNoteCommand(id));
        return HandleResult(result);
    }

    [HttpGet("{id:Guid}", Name = "GetNoteById")]
    public async Task<ActionResult<NotificationResponse>> GetNoteById(Guid id)
    {
        var product = await _mediator.Send(new GetNoteQuery(id));
        return product != null ? Ok(product) : NotFound();
        //TODO добавить тест
    }
}