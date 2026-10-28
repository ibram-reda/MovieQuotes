namespace MovieQuotes.UI.Features.Study;

using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.UI.Services;
using MovieQuotes.UI.ViewModels;

public partial class StudySummaryViewModel : PageViewModelBase
{
    public override string Title => "Study Summary";

    public int TotalPhrases { get; private set; }
    public int CorrectAnswers { get; private set; }
    public int IncorrectAnswers { get; private set; }
    public int UnansweredPhrases => Math.Max(0, TotalPhrases - CorrectAnswers - IncorrectAnswers);
    public bool HasUnansweredPhrases => UnansweredPhrases > 0;
    public double AccuracyRate => TotalPhrases == 0 ? 0 : (double)CorrectAnswers / TotalPhrases * 100;
    public string AccuracyText => $"{AccuracyRate:F0}%";

    [Obsolete("For design-time use only")]
    public StudySummaryViewModel() : base()
    {
        if (!Design.IsDesignMode)
            throw new Exception("This constructor is for design mode only.");
    }

    public StudySummaryViewModel(NavigationService navigationService)
        : base(navigationService)
    {
    }

    public override Task InitAsync(object? parameter = null)
    {
        if (parameter is not StudySummaryParameters summary)
            throw new ArgumentException("Study summary parameters are required.", nameof(parameter));

        if (summary.TotalPhrases < 0 ||
            summary.CorrectAnswers < 0 ||
            summary.IncorrectAnswers < 0 ||
            summary.CorrectAnswers + summary.IncorrectAnswers > summary.TotalPhrases)
        {
            throw new ArgumentOutOfRangeException(nameof(parameter), "Study summary counts must be non-negative and cannot exceed the total phrase count.");
        }

        TotalPhrases = summary.TotalPhrases;
        CorrectAnswers = summary.CorrectAnswers;
        IncorrectAnswers = summary.IncorrectAnswers;
        OnPropertyChanged(nameof(TotalPhrases));
        OnPropertyChanged(nameof(CorrectAnswers));
        OnPropertyChanged(nameof(IncorrectAnswers));
        OnPropertyChanged(nameof(UnansweredPhrases));
        OnPropertyChanged(nameof(HasUnansweredPhrases));
        OnPropertyChanged(nameof(AccuracyRate));
        OnPropertyChanged(nameof(AccuracyText));

        return Task.CompletedTask;
    }

    [RelayCommand]
    private void ReturnToStudyOverview()
    {
        NavigationService.NavigateTo<StudyOverviewViewModel>();
    }

    public sealed class StudySummaryParameters
    {
        public int TotalPhrases { get; init; }
        public int CorrectAnswers { get; init; }
        public int IncorrectAnswers { get; init; }
    }
}
