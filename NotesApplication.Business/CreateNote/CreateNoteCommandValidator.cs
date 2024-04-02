using FluentValidation;
using NotesApplication.Core.Constants;

namespace NotesApplication.Business.CreateNote;

public sealed class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationConst.EmptyName)
            .MaximumLength(ValidationConst.MaxNameLength).WithMessage("Херовое имя пользователя");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationConst.EmptyDescription)
            .MaximumLength(ValidationConst.MaxNameLength).WithMessage("Херовое описание");
    }
}