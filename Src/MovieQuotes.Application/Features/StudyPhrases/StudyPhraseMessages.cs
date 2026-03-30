namespace MovieQuotes.Application.Features.StudyPhrases;
internal static class StudyPhraseMessages
{
    public const string PhraseNotFound = "Study phrase with id {0} was not found.";
    public const string PageNotFound = "could not find page number {0}, the max page number is {1}";
    public const string FailedToUpdate = "No changes were made to the study phrase.";
    public const string InvalidData = "Invalid input data.";
    public const string RequiredPhraseId = "PhraseId is required, study content should be belonge for an existing subtitle Phrase.";

    public const string RequiredStudyId = "StudyId is Required";
    public const string ContentShouldBeEmptyToDelete = "To Delete Phrase Content Should be empty first";
}
