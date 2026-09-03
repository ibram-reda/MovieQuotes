namespace MovieQuotes.Application.Features.Study;

using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieQuotes.Application.Common.Models;
using MovieQuotes.Domain.Models;
using MovieQuotes.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class GetStudyOverviewQueryHandler : IRequestHandler<GetStudyOverviewQuery, OperationResult<StudyOverview>>
{
    private readonly MovieQuotesDbContext dbContext;

    public GetStudyOverviewQueryHandler(MovieQuotesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<OperationResult<StudyOverview>> Handle(GetStudyOverviewQuery request, CancellationToken cancellationToken)
    {
        var result = new OperationResult<StudyOverview>();

        try
        {
            // Get all active study cards
            var activeCards = await this.dbContext.StudyCards
                .Where(c => c.IsActive)
                .ToListAsync(cancellationToken);

            // Get all card progresses
            var allProgresses = await this.dbContext.CardProgresses
                .Where(p => activeCards.Select(c => c.Id).Contains(p.CardId))
                .ToListAsync(cancellationToken);

            // Total number of active cards
            var totalCards = activeCards.Count;

            // Cards due for review today
            var cardsDueToday = allProgresses
                .Count(p => p.NextReviewAt <= DateTime.Now);

            // Total reviews completed
            var totalReviewsCompleted = allProgresses
                .Sum(p => p.ReviewCount);

            // Total lapses (failures)
            var totalLapses = allProgresses
                .Sum(p => p.LapseCount);

            // Success rate calculation
            var successRate = totalReviewsCompleted > 0
                ? (double)(totalReviewsCompleted - totalLapses) / totalReviewsCompleted * 100
                : 0;

            // Current streak (longest consecutive successes in progress)
            var currentStreak = allProgresses
                .Where(p => p.Repetitions > 0)
                .DefaultIfEmpty()
                .Max(p => p?.Repetitions ?? 0);

            // Longest streak achieved (looking at current repetitions as proxy)
            var longestStreak = allProgresses
                .Where(p => p.Repetitions > 0)
                .DefaultIfEmpty()
                .Max(p => p?.Repetitions ?? 0);

            // Average ease factor
            var averageEaseFactor = allProgresses.Count > 0
                ? allProgresses.Average(p => p.EaseFactor)
                : 0;

            // Last reviewed date
            var lastReviewedAt = allProgresses
                .Where(p => p.LastReviewedAt.HasValue)
                .Max(p => p.LastReviewedAt);

            // Recognition exercise statistics
            var recognitionProgresses = allProgresses
                .Where(p => p.ExerciseType == StudyExerciseType.Recognition)
                .ToList();
            
            var recognitionCards = recognitionProgresses.Select(p => p.CardId).Distinct().Count();
            var recognitionReviews = recognitionProgresses.Sum(p => p.ReviewCount);
            var recognitionLapses = recognitionProgresses.Sum(p => p.LapseCount);
            var recognitionSuccessRate = recognitionReviews > 0
                ? (double)(recognitionReviews - recognitionLapses) / recognitionReviews * 100
                : 0;

            // Context recall exercise statistics
            var contextRecallProgresses = allProgresses
                .Where(p => p.ExerciseType == StudyExerciseType.ContextRecall)
                .ToList();
            
            var contextRecallCards = contextRecallProgresses.Select(p => p.CardId).Distinct().Count();
            var contextRecallReviews = contextRecallProgresses.Sum(p => p.ReviewCount);
            var contextRecallLapses = contextRecallProgresses.Sum(p => p.LapseCount);
            var contextRecallSuccessRate = contextRecallReviews > 0
                ? (double)(contextRecallReviews - contextRecallLapses) / contextRecallReviews * 100
                : 0;

            // Recognition cards due today
            var recognitionCardsDueToday = recognitionProgresses
                .Count(p => p.NextReviewAt <= DateTime.Now);

            // Context recall cards due today
            var contextRecallCardsDueToday = contextRecallProgresses
                .Count(p => p.NextReviewAt <= DateTime.Now);

            result.Payload = new StudyOverview
            {
                TotalCards = totalCards,
                CardsDueToday = cardsDueToday,
                TotalReviewsCompleted = totalReviewsCompleted,
                TotalLapses = totalLapses,
                SuccessRate = Math.Round(successRate, 2),
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                AverageEaseFactor = Math.Round(averageEaseFactor, 2),
                LastReviewedAt = lastReviewedAt,
                RecognitionCards = recognitionCards,
                ContextRecallCards = contextRecallCards,
                RecognitionReviews = recognitionReviews,
                ContextRecallReviews = contextRecallReviews,
                RecognitionSuccessRate = Math.Round(recognitionSuccessRate, 2),
                ContextRecallSuccessRate = Math.Round(contextRecallSuccessRate, 2),
                RecognitionCardsDueToday = recognitionCardsDueToday,
                ContextRecallCardsDueToday = contextRecallCardsDueToday
            };
        }
        catch (Exception ex)
        {
            result.AddException(ex);
        }

        return result;
    }
}
