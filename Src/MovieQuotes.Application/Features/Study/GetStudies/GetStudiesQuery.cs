namespace MovieQuotes.Application.Features.Study;
using MovieQuotes.Application.Common.Models; 

public enum StudyType
{
     ContextRecall,
     Recognition,
}

public class GetStudiesQuery
{

    public GetStudiesQuery(StudyType studyType)
    {
        StudyType = studyType;
    }
    public StudyType StudyType { get; set; } 
    public int ItemPerPage { get; set; }   = 400;
    public int PageNumber { get; set; } = 1;
}