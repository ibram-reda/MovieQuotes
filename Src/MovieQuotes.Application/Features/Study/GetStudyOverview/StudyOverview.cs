namespace MovieQuotes.Application.Features.Study;

/// <summary>
/// Represents an overview of study statistics and progress.
/// </summary>
public class StudyOverview
{
    /// <summary>
    /// Total number of active study cards.
    /// </summary>
    public int TotalCards { get; set; }

    /// <summary>
    /// Number of cards due for review today.
    /// </summary>
    public int CardsDueToday { get; set; }

    /// <summary>
    /// Total number of reviews completed across all cards.
    /// </summary>
    public int TotalReviewsCompleted { get; set; }

    /// <summary>
    /// Number of times user failed to recall phrases correctly.
    /// </summary>
    public int TotalLapses { get; set; }

    /// <summary>
    /// Success rate as a percentage (0-100).
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Number of consecutive successful reviews across all cards (current streak).
    /// </summary>
    public int CurrentStreak { get; set; }

    /// <summary>
    /// Longest streak achieved across all cards.
    /// </summary>
    public int LongestStreak { get; set; }

    /// <summary>
    /// Average ease factor across all cards.
    /// </summary>
    public double AverageEaseFactor { get; set; }

    /// <summary>
    /// Last date when a review was completed.
    /// </summary>
    public DateTime? LastReviewedAt { get; set; }

    /// <summary>
    /// Number of recognition exercise cards (multiple choice type).
    /// </summary>
    public int RecognitionCards { get; set; }

    /// <summary>
    /// Number of context recall exercise cards (fill-in-the-blank type).
    /// </summary>
    public int ContextRecallCards { get; set; }

    /// <summary>
    /// Total reviews completed for recognition exercises.
    /// </summary>
    public int RecognitionReviews { get; set; }

    /// <summary>
    /// Total reviews completed for context recall exercises.
    /// </summary>
    public int ContextRecallReviews { get; set; }

    /// <summary>
    /// Success rate for recognition exercises (0-100).
    /// </summary>
    public double RecognitionSuccessRate { get; set; }

    /// <summary>
    /// Success rate for context recall exercises (0-100).
    /// </summary>
    public double ContextRecallSuccessRate { get; set; }

    /// <summary>
    /// Number of recognition exercise cards due for review today.
    /// </summary>
    public int RecognitionCardsDueToday { get; set; }

    /// <summary>
    /// Number of context recall exercise cards due for review today.
    /// </summary>
    public int ContextRecallCardsDueToday { get; set; }
}
