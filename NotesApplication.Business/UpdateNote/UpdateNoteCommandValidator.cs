using FluentValidation;

namespace NotesApplication.Business.UpdateNote;

public sealed class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NewName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NewDescription).NotEmpty().MaximumLength(100);
    }
}