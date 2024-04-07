using FluentValidation;

namespace NotesApplication.Business.GetNote;

public sealed class GetNoteQueryValidator : AbstractValidator<GetNoteQuery>
{
    public GetNoteQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Пустой Guid");
    }
}