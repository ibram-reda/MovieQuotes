namespace MovieQuotes.Domain.Models;

public class StudyMaterialPhrase
{
    private StudyMaterialPhrase() { }

    public static StudyMaterialPhrase Create(int studyMaterialId, int phraseId, int sequance, string? arabicTranslation = null)
    {
        return new StudyMaterialPhrase
        {
            StudyMaterialId = studyMaterialId,
            PhraseId = phraseId,
            Sequance = sequance,
            ArabicTranslation = arabicTranslation
        };
    }

    public int StudyMaterialId { get; private set; }
    public StudyMaterial StudyMaterial { get; private set; } = null!;

    public int PhraseId { get; private set; }
    public SubtitlePhrase Phrase { get; private set; } = null!;

    public string? ArabicTranslation { get; private set; }

    public string? Content {get; set;}

    public int Sequance {get; private set;}
}