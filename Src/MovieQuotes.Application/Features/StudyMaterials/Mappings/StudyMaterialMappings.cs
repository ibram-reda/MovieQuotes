namespace MovieQuotes.Application.Features.StudyMaterials; 

internal static class StudyMaterialMappings
{
    internal static StudyMaterial ToStudyMaterial(this Domain.Models.StudyMaterial source)
    {
        return new StudyMaterial
        {
            Id = source.Id,
            PartOfSpeech = source.PartOfSpeech,
            Content = source.Content,
            ContentArabicTranslation = source.ContentArabicTranslation,
            Origin = source.Origin,
            Definition = source.Definition,
            ArPhraseTranslation = source.ArPhraseTranslation,
            SrcPhrase = source.Phrase!.Text,
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