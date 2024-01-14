using FluentValidation;
using todo_app.core.DTOs;

namespace todo_app.core.Models.Utils;

public class TitleOrBodyValidator : AbstractValidator<NoteDTO>
{
    public TitleOrBodyValidator()
    {
        RuleFor(n => n.Title)
            .NotEmpty()
            .When(n => String.IsNullOrWhiteSpace(n.Body))
            .WithMessage("note must have a title or a body or both");
        RuleFor(n => n.Body)
            .NotEmpty()
            .When(n => String.IsNullOrWhiteSpace(n.Title))
            .WithMessage("note must have a title or a body or both");
    }
}
