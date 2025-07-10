namespace MovieQuotes.Domain.Models;
public class StudyPhrase
{
    private StudyPhrase()
    {
    }
    public int Id { get; private set; }
    public int PhraseId { get; private set; }
    public virtual SubtitlePhrase? Phrase { get; private set; }

    /// <summary>
    /// can be one of the following
    /// (Word,Idiom,PhrasalVerb...)
    /// </summary>
    public string? StudyType { get; private set; }

    /// <summary>
    /// what you learn from this subtitle phrase.
    /// </summary>
    public string? Content { get; private set; }

    /// <summary>
    /// what is the translation of the content part.
    /// </summary>
    public string? Translation { get; private set; }

    public DateTime AddedDate { get; private set; } = DateTime.Now;

    public static StudyPhrase CreateStudyPhrase(int phraseId,
        string? studyType,
        string? content,
        string? translation)
    {
        return new StudyPhrase
        {
            PhraseId = phraseId,
            StudyType = studyType,
            Content = content,
            Translation = translation,
        };
    }

    public static StudyPhrase CreateStudyPhrase(SubtitlePhrase phrase,
        string? studyType,
        string? content,
        string? translation)
    {
        return StudyPhrase.CreateStudyPhrase(phrase.Id, studyType, content, translation);
    }

    public void EditContent(string? content)
    {
        if(content == this.Content)
            return;
        this.Content = content;
    }

    public void EditStudyType(string? studyType)
    {
        if(studyType == this.StudyType)
            return;
        this.StudyType = studyType;
    }

    public void EditTranslation(string? translation)
    {
        if(translation == this.Translation)
            return;
        this.Translation = translation;
    }
}
