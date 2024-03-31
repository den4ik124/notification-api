﻿using MediatR;

namespace NotesApplication.Business.CreateNote;

public class CreateNoteCommand : IRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}