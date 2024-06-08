using MediatR;
using Microsoft.AspNetCore.Authorization;
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
[Authorize(Policy = "User")]
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
        var result = await _mediator.Send(new UpdateNoteCommand
        {
            Id = id,
            NewDescription = request.Description,
            NewName = request.Name
        });
        return HandleResult(result);
    }

    [HttpGet()]
    public async Task<IEnumerable<NotificationResponse>> GetAllNotes()
    {
        return await _mediator.Send(new GetAllNotesQuery());
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)] // StatusCode(201);
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNotification(CreateNoteCommand request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteNoteCommand(id));
        return HandleResult(result);
    }

    [HttpGet("{id:Guid}", Name = "GetNoteById")]
    public async Task<ActionResult<NotificationResponse>> GetNoteById(Guid id)
    {
        var product = await _mediator.Send(new GetNoteQuery(id));
        return product != null ? Ok(product) : NotFound();
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
}