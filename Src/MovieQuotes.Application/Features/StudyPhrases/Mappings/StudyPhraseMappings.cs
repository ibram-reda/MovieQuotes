namespace MovieQuotes.Application.Features.StudyPhrases.Mappings;

using MovieQuotes.Application.Features.StudyPhrases.Models;


internal static class StudyPhraseMappings
{
    internal static StudyPhrase ToStudyPhrase(this Domain.Models.StudyPhrase src)
    {
        return new StudyPhrase
        {
            StudyId = src.Id,
            PhraseId = src.PhraseId,
            PhraseText = src.Phrase?.Text ?? "",
            MovieName = src.Phrase?.Movie?.Title ?? "",
            StartTime = src.Phrase?.StartTime ?? TimeSpan.Zero,
            EndTime = src.Phrase?.EndTime ?? TimeSpan.Zero,
            VideoLocation = src.Phrase?.GetVideoClipPath() ?? string.Empty,
            Content = src.Content,
            Translation = src.Translation,
            StudyType = src.StudyType,
            ArContentTranslation = src.ArContentTranslation,
            PhraseArTranslation = src.ArPhraseTranslation,
            Origin = src.Origin,
            Notes = src.Notes,
            NextReviewDate = src.Progress?.NextReviewDate ?? DateTime.Now,
            IsDraft = src.IsDraft,
            Examples = src.Examples,
            Synonyms = src.Synonyms,
            Level = src.Level,
            Pronunciation = src.Pronunciation
            };
    }
}
