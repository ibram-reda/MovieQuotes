namespace MovieQuotes.Application.Features.Study; 

internal static class StudyMaterialMappings
{
    internal static StudyMaterial ToStudyMaterial(this Domain.Models.StudyMaterial source)
    {
        return new StudyMaterial
        {
            Id = source.Id,
            PhraseId = source.PhraseId,
            PartOfSpeech = source.PartOfSpeech,
            Content = source.Content,
            ContentArabicTranslation = source.ContentArabicTranslation,
            Origin = source.Origin,
            Definition = source.Definition,
            ArPhraseTranslation = source.ArPhraseTranslation,
            IsDraft = source.IsDraft,
            Examples = source.Examples,
            Synonyms = source.Synonyms,
            Level = source.Level,
            Pronunciation = source.Pronunciation,
            IsVulgar = source.IsVulgar,
            Notes = source.Notes,
            Tags = source.Tags,
        };
    }
}