namespace MovieQuotes.Domain.Models;

public class StudySession
{
    private StudySession()
    {
    }

    public int Id { get; private set; }

    public StudyExerciseType ExerciseType { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public int CurrentCardPosition { get; private set; }

    public ICollection<StudySessionCard> Cards { get; private set; } = [];

    public static StudySession Start(
        StudyExerciseType exerciseType,
        IEnumerable<int> studyCardIds,
        DateTime? startedAt = null)
    {
        ArgumentNullException.ThrowIfNull(studyCardIds);

        var cardIds = studyCardIds.ToList();
        if (cardIds.Count == 0)
            throw new ArgumentException("A study session must contain at least one card.", nameof(studyCardIds));

        var session = new StudySession
        {
            ExerciseType = exerciseType,
            StartedAt = startedAt ?? DateTime.Now,
            CurrentCardPosition = 0
        };

        for (var order = 0; order < cardIds.Count; order++)
            session.Cards.Add(StudySessionCard.Create(cardIds[order], order));

        return session;
    }

    public StudySessionCard GetCurrentCard()
    {
        EnsureInProgress();
        return Cards.OrderBy(card => card.Order).ElementAt(CurrentCardPosition);
    }

    public void CompleteCurrentCard(DateTime? completedAt = null)
    {
        GetCurrentCard().Complete(completedAt);
    }

    public void MoveToNextCard()
    {
        EnsureInProgress();

        if (!GetCurrentCard().IsCompleted)
            throw new InvalidOperationException("The current card must be completed before moving to the next card.");

        if (CurrentCardPosition >= Cards.Count - 1)
            throw new InvalidOperationException("The current card is the last card in the session.");

        CurrentCardPosition++;
    }

    public void CompleteSession(DateTime? completedAt = null)
    {
        EnsureInProgress();

        if (Cards.Any(card => !card.IsCompleted))
            throw new InvalidOperationException("All session cards must be completed before completing the session.");

        CompletedAt = completedAt ?? DateTime.Now;
    }

    private void EnsureInProgress()
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("The study session is already completed.");
    }
}