namespace MovieQuotes.Domain.Models;

public enum ReviewQuality
{
    CompleteFailure = 0,
    Incorrect = 1,
    Difficult = 2,
    Correct = 3,
    Good = 4,
    Perfect = 5
}
public enum StudyExerciseType
{
    Recognition,
    ContextRecall
}

public class CardProgress
{
    private CardProgress() { }

    public int Id { get; private set; }

    public int CardId { get; private set; }
    public StudyCard Card { get; private set; }

    public StudyExerciseType ExerciseType { get; private set; }

    public DateTime? LastReviewedAt { get; private set; }

    public DateTime NextReviewAt { get; private set; } 

    /// <summary>
    /// Number of successful reviews in a row. It resets to 0 if the user fails to recall the phrase  (quality < 3).
    /// </summary>

    public int Repetitions { get; private set; }

    public int IntervalDays { get; private set; }

    /// <summary>
    /// A factor that determines how quickly the interval increases (Difficulty score). 
    /// It is adjusted based on the user's performance, 
    /// with a minimum value of 1.3 to prevent intervals from becoming too short.
    /// </summary>

    public double EaseFactor { get; private set; } = 2.5;

    /// <summary>
    /// The number of times the user has reviewed the phrase, regardless of success or failure.
    /// </summary>
    public int ReviewCount { get; private set; }

    /// <summary>
    /// The number of times the user has failed to recall the phrase correctly (quality < 3).
    /// </summary>
    public int LapseCount { get; private set; }

    public DateTime CreatedDate { get; private set; } = DateTime.Now;

    public static CardProgress Create(int cardId,StudyExerciseType exerciseType)
    {
        return new CardProgress
        {
            CardId = cardId,
            ExerciseType = exerciseType,
            NextReviewAt = DateTime.Now
        };
    }
    public static CardProgress Create(StudyExerciseType exerciseType)
    {
        return new CardProgress
        { 
            ExerciseType = exerciseType,
            NextReviewAt = DateTime.Now
        };
    }


    public void Review(ReviewQuality reviewQuality)
    {
        LastReviewedAt = DateTime.Now;
        ReviewCount++;

        var quality = (int)reviewQuality;
        if (quality < 3)
        {
            Repetitions = 0;
            IntervalDays = 1;
            LapseCount++;
        }
        else
        {
            if (Repetitions == 0)
                IntervalDays = 1;
            else if (Repetitions == 1)
                IntervalDays = 6;
            else
                IntervalDays = (int)Math.Round(IntervalDays * EaseFactor);

            Repetitions++;
        }

        EaseFactor = Math.Max(
            1.3,
            EaseFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02))
        );

        NextReviewAt = DateTime.Now.AddDays(IntervalDays);
    }
}