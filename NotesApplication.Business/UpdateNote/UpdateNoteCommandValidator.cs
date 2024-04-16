using FluentValidation;
using NotesApplication.Core.Constants;

namespace NotesApplication.Business.UpdateNote;

public sealed class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Пустой Guid");
        RuleFor(x => x.NewName)
            .NotEmpty().WithMessage(ValidationConst.EmptyName)
            .MaximumLength(ValidationConst.MaxNameLength).WithMessage("Херовое имя пользователя");
        RuleFor(x => x.NewDescription)
            .NotEmpty().WithMessage(ValidationConst.EmptyDescription)
            .MaximumLength(ValidationConst.MaxNameLength).WithMessage("Херовое описание");
    }
}