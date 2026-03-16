namespace MovieQuotes.Domain.Validators;

using System.Data;
using FluentValidation;
using MovieQuotes.Domain.Models;

public class MovieValidator: AbstractValidator<Movie>
{
    public MovieValidator()
    {
        RuleFor(a => a.Title)
            .NotNull().WithMessage("Title is Required")
            .NotEmpty().WithMessage("Title can not be Empty")
            .MaximumLength(150).WithMessage("Title Can be at most 150 characters");

        RuleFor(a => a.FolderName)
            .NotNull().WithMessage("Folder Name is Required")
            .NotEmpty().WithMessage("Folder Name can not be Empty")
            .MaximumLength(150).WithMessage("Folder Name Can be at most 150 characters");

        RuleFor(a => a.NameId)
            .NotNull().WithMessage("NameId is Required")
            .NotEmpty().WithMessage("NameId can not be Empty")
            .MaximumLength(150).WithMessage("NameId Can be at most 150 characters")
            .Matches(@"^[\w-]+$").WithMessage("NameId should be only alphanumeric or hyphen(-)");


        RuleFor(a => a.IMDBId)
            .MaximumLength(12).WithMessage("IMDB Id Can be at most 12 characters");

        RuleFor(a => a.Description)
            .MaximumLength(700).WithMessage("Description should contains at most 700 characters long");

        
        RuleFor(a => a.BaseFolderDir)
            .NotNull().WithMessage("Base Folder Directory is Required")
            .NotEmpty().WithMessage("Base Folder Directory can not be Empty")
            .MaximumLength(400).WithMessage("Base Folder Directory can only contains 400 char at most")
            .When(m => !Directory.Exists(m.BaseFolderDir)).WithMessage("Base Folder Directory should be valid directory on system!");


        RuleFor(a=> a.VideoFilePath)
            .MaximumLength(200).WithMessage("Video File Path can only contains 200 char at most")
            .When(m => !string.IsNullOrEmpty(m.VideoFilePath) && !File.Exists(Path.Combine(m.BaseFolderDir,m.FolderName, m.VideoFilePath))).WithMessage("Video File Path should be file on system!");
        
        RuleFor(a => a.CoverFilePath)
            .MaximumLength(200).WithMessage("Cover File Path can only contains 200 char at most")
            .When(m => !string.IsNullOrEmpty(m.CoverFilePath) && !File.Exists(Path.Combine(m.BaseFolderDir,m.FolderName, m.CoverFilePath))).WithMessage("Cover File Path should be file on system!");
         
    }
}
