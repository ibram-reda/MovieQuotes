namespace MovieQuotes.Application.Features.AI.GenerateAISuggestionForMaterials;

using MediatR;
using MovieQuotes.AI;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Interfaces;

public sealed class GenerateAISuggestionForMaterialsCommandHandler : IRequestHandler<GenerateAISuggestionForMaterialsCommand, OperationResult<bool>>
{
    private readonly IMovieQUnitOfWork unitOfWork;
    private readonly AppSettings appSettings;

    public GenerateAISuggestionForMaterialsCommandHandler(IMovieQUnitOfWork unitOfWork, AppSettings appSettings)
    {
        this.unitOfWork = unitOfWork;
        this.appSettings = appSettings;
    }

    public async Task<OperationResult<bool>> Handle(GenerateAISuggestionForMaterialsCommand request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<bool>();

        try
        {
            // Get Draft Study Materials without AI Tags
            var draftStudyMaterials = await this.unitOfWork.StudyMaterials.GetAllStudyMaterialsWithPhraseAsync(cancellationToken);
            var llm = new LLMService(this.appSettings.OllamaApiUrl, this.appSettings.OllamaModelName);
            foreach (var st in draftStudyMaterials.Reverse<Domain.Models.StudyMaterial>())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    result.AddException(new OperationCanceledException("Operation was canceled."));
                    return result;
                }
                try
                {

                    var res = await llm.GenerateAISuggestionForStudyMaterial(st.Id, st.Phrase?.Text ?? "", st.Content ?? "", cancellationToken);
                    if (res != null)
                    {
                        st.AddTag("AI");
                        if(string.IsNullOrEmpty(st.Level))
                        {
                            st.EditLevel(res.Level);
                        }
                        await this.unitOfWork.StudyMaterials.UpdateAsync(st);
                        await this.unitOfWork.SaveAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    result.AddException(ex);
                    continue;
                }

            }
        }
        catch (Exception ex)
        {
            result.AddException(ex);
        }

        return result;
    }
}