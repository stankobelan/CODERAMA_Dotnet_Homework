namespace CoderamaInterview.Validators;
using CoderamaInterview.Models;
using FluentValidation;

public class DocumentValidator : AbstractValidator<Document>
{
    public DocumentValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Document ID is required.");
        RuleFor(x => x.Tags)
            .NotNull().WithMessage("At least one tag is required.")
            .Must(tags => tags.Any()).WithMessage("At least one tag must be provided.");
        RuleFor(x => x.Data).NotNull().WithMessage("Data is required.");
    }
}