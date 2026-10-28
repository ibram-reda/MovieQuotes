namespace MovieQuotes.Application.Features.StudyMaterials.GetStudyMaterialLevels;

public class LevelFilter(string level, int count)
{
    public string Level { get; } = level;
    public int Count { get; } = count;
};
