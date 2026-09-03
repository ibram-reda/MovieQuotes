namespace MovieQuotes.UI.Features.Study;

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using MovieQuotes.UI.Services;

public partial class ActiveRecallView : UserControl
{
    private readonly DispatcherTimer _timer; //create timer 
    private int elapsedTime = 0;
    public ActiveRecallView()
    {
        InitializeComponent();
        //Set timer Interval and start
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();

    }
    private void OnTimerTick(object? sender, EventArgs e)
    {
        elapsedTime++;
        TxtTime.Text = $"Elapsed Time: {elapsedTime}m";
    }

    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (this.DataContext is ActiveRecallViewModel vm)
        {
            vm.InitCommand.Execute(null);
            vm.PropertyChanged += (s, e) =>
            {
                
                if (e.PropertyName == nameof(vm.SpellingAnalysis))
                {
                    RenderAnswer(vm.SpellingAnalysis?.Characters ?? new List<SpellingCharacter>());
                }
            };
        }
    }

    private void RenderAnswer(IReadOnlyList<SpellingCharacter> characters)
    {
        AnswerTextBlock.Inlines.Clear();
        CorrectTextBlock.Inlines.Clear();

        IBrush errorBrush = this.TryFindResource("ErrorStatusColor", out var errorColor) ?
        SolidColorBrush.Parse(errorColor?.ToString() ?? "") : Brushes.Red;

        IBrush? CorrectBrush = this.TryFindResource("CorrectGreenBrush", out var correctColor) ? correctColor as IBrush: Brushes.Green;

        foreach (var character in characters)
        {

            var run = new Run
            {
                Text = character.Actual?.ToString() ?? "-"
            };
            var correctRun = new Run
            {
                Text = character.Expected?.ToString() ?? "-",
                Foreground = CorrectBrush,
            }; 

            switch (character.State)
            {
                case SpellingCharacterState.Correct:
                    run.Foreground = CorrectBrush;
                    break;

                case SpellingCharacterState.Substituted:
                case SpellingCharacterState.Extra:
                case SpellingCharacterState.Missing:
                    run.Foreground = errorBrush;
                    run.FontWeight = FontWeight.Bold;
                    run.TextDecorations = TextDecorations.Underline;
                    correctRun.TextDecorations = TextDecorations.Underline;
                    break;
            }

            AnswerTextBlock.Inlines.Add(run);
            AnswerTextBlock.Inlines.Add(new Run { Text = " " }); // Add space between characters
            
            CorrectTextBlock.Inlines.Add(correctRun);
            CorrectTextBlock.Inlines.Add(new Run { Text = " " }); // Add space between characters


        }
    }
}