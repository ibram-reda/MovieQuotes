namespace MovieQuotes.Application.Features.StudyMaterials.GetStudyMaterials;
using MovieQuotes.Application.Common.Models;

public enum StudyMaterialSortOrder
{
    ModifiedDate,
    CreatedDate,
    Alphabetical
}

public class GetStudyMaterialsQuery
{
    public int MovieId { get; init; }
    public string? Level { get; init; }
    public string? PartOfSpeech { get; init; }
    public string? SearchText { get; init; }
    public bool DraftOnly { get; init; }
    public bool VulgarOnly { get; init; }
    public uint ResultPerPage { get; init; } = 10;
    public uint PageNumber { get; init; }
    public StudyMaterialSortOrder SortOrder { get; init; } = StudyMaterialSortOrder.ModifiedDate;
}
