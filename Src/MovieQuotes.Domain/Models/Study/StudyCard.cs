namespace MovieQuotes.Domain.Models;



public class StudyCard
{
    private StudyCard()
    {
    }

    public int Id { get; private set; }

    public int StudyMaterialId { get; private set; }

    public virtual StudyMaterial StudyMaterial { get; private set; } = null!;

 

    public bool IsActive { get; private set; }

    public ICollection<CardProgress> Progresses { get; private set; } = [];

    public DateTime CreatedAt { get; private set; }

    public DateTime ModifiedAt { get; private set; }

    public static StudyCard Create(int studyMaterialId)
    {
        var card = new StudyCard
        {
            StudyMaterialId = studyMaterialId,
            IsActive = true,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };

        
    

        return card;
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