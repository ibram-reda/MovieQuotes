namespace MovieQuotes.Domain.Models;

public class StudyMaterial
{
    private StudyMaterial()
    {
    }
    public int Id { get; private set; }
    public int PhraseId { get; private set; }
    public virtual SubtitlePhrase? Phrase { get; private set; }


    /// <summary>
    /// can be one of the following
    /// (Word,Idiom,PhrasalVerb...)
    /// </summary>
    public string? PartOfSpeech { get; private set; }

    /// <summary>
    /// what you learn from this subtitle phrase.
    /// </summary>
    public string? Content { get; private set; }

    /// <summary>
    /// Gets the Arabic translation of the content.
    /// </summary>
    public string? ContentArabicTranslation { get; private set; }

    /// <summary>
    /// what is the origin of this content (optional).
    /// </summary>
    public string? Origin { get; private set; }

    

    /// <summary>
    /// English defintion of the content.
    /// </summary>
    public string? Definition { get; private set; }

    /// <summary>
    /// Gets the Arabic translation for the associated Phrase
    /// </summary>
    public string? ArPhraseTranslation { get; private set; }

    public bool IsDraft { get; private set; } = false;
    public string Examples { get; private set; } = string.Empty;
    public string Synonyms { get; private set; } = string.Empty;

    public string Level { get; private set; } = string.Empty;
    public string Pronunciation { get; private set; } = string.Empty;

    public bool IsVulgar { get; private set; } = false;

    /// <summary>
    /// Gets the notes associated with this instance.
    /// </summary>
    public string? Notes { get; private set; }

    public List<StudyCard> StudyCards {get;} = new();

    /// <summary>
    /// coma seperated Tags
    /// </summary>
    public string? Tags { get; private set; }
    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public DateTime ModifiedDate { get; private set; } = DateTime.Now;
    // ----------------- public StudyPhraseProgress? Progress { get; private set; }

    public static StudyMaterial CreateStudyMaterial(int phraseId,
        string? partOfSpeach,
        string? content,
        string? defintion,
        string? contentArabicTranslation,
        string? arPhraseTranslation,
        string? origin,
        string? notes,
        bool isDraft = false,
        string examples = "",
        string synonyms = "",
        string level = "",
        string pronunciation = "",
        bool isVulgar=false)
    {
        return new StudyMaterial
        {
            PhraseId = phraseId,
            PartOfSpeech = partOfSpeach,
            Content = content,
            Definition = defintion?.Trim(),
            ContentArabicTranslation = contentArabicTranslation?.Trim(),
            ArPhraseTranslation = arPhraseTranslation,
            Origin = origin,
            Notes = notes,
            IsDraft = isDraft,
            Examples = PutDashInStartingLines(examples),
            Synonyms = synonyms,
            Level = level,
            Pronunciation = pronunciation?.Trim(),
            IsVulgar=isVulgar,
        };
    }


    public void EditContent(string? content)
    {
        if (content == this.Content)
            return;
        this.Content = content;
        ModifiedDate = DateTime.Now;
    }

    public void EditPartOfSpeach(string? partOfSpeach)
    {
        if (partOfSpeach == this.PartOfSpeech)
            return;
        this.PartOfSpeech = partOfSpeach;
        ModifiedDate = DateTime.Now;
    }

    public void EditDefination(string? defintion)
    {
        if (defintion == this.Definition)
            return;
        this.Definition = defintion?.Trim();
        ModifiedDate = DateTime.Now;
    }

    public void EditContentArabicTranslation(string? contentArabicTranslation)
    {
        if (contentArabicTranslation == this.ContentArabicTranslation)
            return;
        this.ContentArabicTranslation = contentArabicTranslation;
        ModifiedDate = DateTime.Now;
    }

    public void EditArPhraseTranslation(string? arPhrase)
    {
        if (arPhrase == this.ArPhraseTranslation)
            return;
        this.ArPhraseTranslation = arPhrase;
        ModifiedDate = DateTime.Now;
    }

    public void EditOrigin(string? origin)
    {
        if (origin == this.Origin)
            return;
        this.Origin = origin;
        ModifiedDate = DateTime.Now;
    }

    public void EditNotes(string? notes)
    {
        if (notes == this.Notes)
            return;
        this.Notes = notes;
        ModifiedDate = DateTime.Now;
    }

    public void EditExamples(string examples)
    {
        if (examples == this.Examples)
            return;
        this.Examples = PutDashInStartingLines(examples);
        ModifiedDate = DateTime.Now;
    }

    public void EditSynonyms(string synonyms)
    {
        if (synonyms == this.Synonyms)
            return;
        this.Synonyms = synonyms;
        ModifiedDate = DateTime.Now;
    }

    public void MarkAsDraft()
    {
        this.IsDraft = true;
        ModifiedDate = DateTime.Now;
    }

    public void MarkAsReady()
    {
        this.IsDraft = false;
        ModifiedDate = DateTime.Now;
    }

    public void EditLevel(string level)
    {
        if (level == this.Level)
            return;
        this.Level = level;
        ModifiedDate = DateTime.Now;
    }

    public void EditPronunciation(string pronunciation)
    {
        if (pronunciation == this.Pronunciation)
            return;
        this.Pronunciation = pronunciation.Trim();
        ModifiedDate = DateTime.Now;
    }


    static string PutDashInStartingLines(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        var lines = text.Split(Environment.NewLine);
        for (int i = 0; i < lines.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(lines[i]) && !lines[i].TrimStart().StartsWith("-"))
            {
                lines[i] = "- " + lines[i].TrimStart();
            }
        }
        return string.Join(Environment.NewLine, lines.Where(l => !string.IsNullOrWhiteSpace(l)));
    }
}
