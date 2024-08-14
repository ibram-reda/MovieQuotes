namespace MovieQuotes.Domain.Validators;

using FluentValidation;
using MovieQuotes.Domain.Models;

public class MovieValidator: AbstractValidator<Movie>
{
    public MovieValidator()
    {
        RuleFor(a => a.Title)
            .NotNull().WithMessage("Title is Required")
            .NotEmpty().WithMessage("Title can not be Empty")
            .MaximumLength(700).WithMessage("Title Can be at most 700 characters");

        RuleFor(a => a.Description)
            .MaximumLength(700).WithMessage("Description should contains at most 700 characters long");

        RuleFor(a => a.LocalPath)
            .MaximumLength(300).WithMessage("LocalPath can only contains 300 char at most");
    }
}
