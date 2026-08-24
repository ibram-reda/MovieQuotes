namespace MovieQuotes.Application.Features.Study;

public class StudyMaterialPhrase
{
    public int PhraseId { get; set; }
    // TODO: Add Content to hilight in phrse;
    public string PhraseText { get; set; } = string.Empty;
    public string ArPhraseTranslation { get; set; } = string.Empty;
    public string MovieCoverUrl { get; set; } = string.Empty;
    public string MovieTitle { get; set; } = string.Empty;
    public int Sequance { get; set; }
}

public class StudyProgress
{
    public string ProgressType { get; set; }

    public int Repetitions { get; set; }
    public int ReviewCount { get; set; }
    public int LapseCount { get; set; }

    public DateTime? LastReviewedAt { get; set; }
    public DateTime NextReviewAt { get; set; }

    public int Strength
    {
        get
        {
            var repetitionScore = Math.Min(Repetitions / 5.0, 1.0);

            var lapsePenalty = Math.Min(LapseCount * 0.05, 0.30);

            var strength = (repetitionScore - lapsePenalty) * 100;

            return (int)Math.Clamp(strength, 0, 100);
        }
    }
    public string Status => Strength switch
    {
        < 20 => "New",
        < 40 => "Learning",
        < 70 => "Developing",
        < 90 => "Familiar",
        _ => "Strong"
    };
}

public class StudyMaterialDetails
{
    public int Id { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Content { get; set; }
    public string? ContentArabicTranslation { get; set; }
    public string? Origin { get; set; }
    public string? Definition { get; set; }

    public List<StudyMaterialPhrase> MaterialPhrases { get; set; } = [];
    public List<StudyProgress> Progresses { get; set; } = [];
    public bool IsCurrentlyLerning { get; set; }
    public bool IsDraft { get; set; }
    public string Examples { get; set; } = string.Empty;
    public string Synonyms { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Pronunciation { get; set; } = string.Empty;
    public bool IsVulgar { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
}