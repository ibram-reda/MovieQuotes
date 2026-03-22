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
    /// what is the origin of this content (optional).
    /// </summary>
    public string? Origin { get; private set; }

    /// <summary>
    /// Gets the Arabic translation of the content.
    /// </summary>
    public string? ArContentTranslation { get; private set; }

    /// <summary>
    /// English defintion of the content.
    /// </summary>
    public string? Translation { get; private set; }

    /// <summary>
    /// Gets the Arabic translation for the associated Phrase
    /// </summary>
    public string? ArPhraseTranslation { get; private set; }

    /// <summary>
    /// Gets the notes associated with this instance.
    /// </summary>
    public string? Notes { get; private set; }
    public DateTime AddedDate { get; private set; } = DateTime.Now;
    public StudyPhraseProgress? Progress { get; private set; }

    public static StudyPhrase CreateStudyPhrase(int phraseId,
        string? studyType,
        string? content,
        string? translation,
        string? arContent,
        string? arPhrase,
        string? origin,
        string? Note)
    {
        return new StudyPhrase
        {
            PhraseId = phraseId,
            StudyType = studyType,
            Content = content,
            Translation = translation,
            ArContentTranslation = arContent,
            ArPhraseTranslation = arPhrase,
            Origin = origin,
            Notes = Note
        };
    }

    public static StudyPhrase CreateStudyPhrase(SubtitlePhrase phrase,
        string? studyType,
        string? content,
        string? translation,
        string? arContent,
        string? arPhrase,
        string? origin,
        string? note)
    {
        return StudyPhrase.CreateStudyPhrase(phrase.Id, studyType, content, translation,arContent,arPhrase,origin,note);
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

    public void EditArContentTranslation(string? arContent)
    {
        if(arContent == this.ArContentTranslation)
            return;
        this.ArContentTranslation = arContent;
    }

    public void EditArPhraseTranslation(string? arPhrase)
    {
        if(arPhrase == this.ArPhraseTranslation)
            return;
        this.ArPhraseTranslation = arPhrase;
    }

    public void EditOrigin(string? origin)
    {
        if(origin == this.Origin)
            return;
        this.Origin = origin;
    }

    public void EditNotes(string? notes)
    {
        if(notes == this.Notes)
            return;
        this.Notes = notes;
    }
}
