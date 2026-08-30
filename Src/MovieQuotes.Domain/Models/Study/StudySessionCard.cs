namespace MovieQuotes.Domain.Models;

public class StudySessionCard
{
    private StudySessionCard()
    {
    }

    public int Id { get; private set; }

    public int StudySessionId { get; private set; }

    public int StudyCardId { get; private set; }

    public int Order { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public static StudySessionCard Create(int studyCardId, int order)
    {
        if (studyCardId <= 0)
            throw new ArgumentOutOfRangeException(nameof(studyCardId));

        if (order < 0)
            throw new ArgumentOutOfRangeException(nameof(order));

        return new StudySessionCard
        {
            StudyCardId = studyCardId,
            Order = order
        };
    }

    public void Complete(DateTime? completedAt = null)
    {
        if (IsCompleted)
            throw new InvalidOperationException("The session card is already completed.");

        IsCompleted = true;
        CompletedAt = completedAt ?? DateTime.Now;
    }
}