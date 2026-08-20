namespace MovieQuotes.Application.Features.Study.AddNewStudyMaterial;

using MediatR;
using MovieQuotes.Application.Common.Models;

public class AddNewStudyMaterialCommand : IRequest<OperationResult<StudyMaterial>>
{
    public int PhraseId { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Content { get; set; }
    public string? Definition { get; set; }
    public string? ContentArabicTranslation { get; set; }
    public string? ArPhraseTranslation { get; set; }
    public string? Origin { get; set; }
    public string? Notes { get; set; }
    public bool IsDraft { get; set; } = true;
    public string Examples { get; set; } = string.Empty;
    public string Synonyms { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Pronunciation { get; set; } = string.Empty;
    public bool IsVulgar { get; set; }
}