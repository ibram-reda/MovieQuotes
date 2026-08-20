namespace MovieQuotes.Domain.Models;

public enum StudyMode
{
    Recognition = 1,
    Listening = 2,
    Reading = 3,
    Writing = 4,
}

public class StudyCard
{
    private StudyCard()
    {
    }

    public int Id { get; private set; }

    public int StudyMaterialId { get; private set; }

    public virtual StudyMaterial StudyMaterial { get; private set; } = null!;

    public StudyMode Mode { get; private set; }

    public bool IsActive { get; private set; }

    public CardProgress CardProgress { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime ModifiedAt { get; private set; }

    public static StudyCard Create(
        int studyMaterialId,
        StudyMode mode)
    {
        return new StudyCard
        {
            StudyMaterialId = studyMaterialId,
            Mode = mode,
            IsActive = true,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };
    }

    public void Deactivate()
    {
        IsActive = false;
        ModifiedAt = DateTime.Now;
    }

    public void Activate()
    {
        IsActive = true;
        ModifiedAt = DateTime.Now;
    }
}