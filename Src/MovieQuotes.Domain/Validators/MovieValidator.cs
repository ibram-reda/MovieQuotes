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
            .MaximumLength(700).WithMessage("Title Can be at most 700 characters")
            .Matches(@"^[\w,\s-()]+$").WithMessage("Title should be apply for folder name");

        RuleFor(a => a.IMDBId)
            .MaximumLength(12).WithMessage("IMDB Id Can be at most 12 characters");

        RuleFor(a => a.Description)
            .MaximumLength(700).WithMessage("Description should contains at most 700 characters long");

        RuleFor(a => a.LocalPath)
            .MaximumLength(700).WithMessage("LocalPath can only contains 700 char at most");
       
        RuleFor(a => a.LocalPath)
            .MaximumLength(700).WithMessage("CoverURL can only contains 700 char at most");
    }
}
