namespace MovieQuotes.UI.Features.StudyVocabs.ReviseVocab;


using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MovieQuotes.Application.Features.MoviePhrases.Models;
using MovieQuotes.Application.Features.StudyPhrases.Models;
using MovieQuotes.UI.ViewModels.Dialogues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class ExportStudy:ObservableObject
{
    public ExportStudy(StudyPhrase phrase)
    {
        this.Phrase = phrase;
    }
    [ObservableProperty] bool isSelected =true;
    public StudyPhrase Phrase;
    [ObservableProperty] string studyPhrase;
}
internal partial class ExportStudiesViewModel : DialogueViewModelBase
{
    [ObservableProperty] bool _isSelectAll = true;
    [ObservableProperty] bool _isInverseSelection;
    [ObservableProperty] bool includeEnDefention;
    [ObservableProperty] bool includePhras;
    [ObservableProperty] bool includePhrasTranslation;
    [ObservableProperty] ExportStudy? selectedStudy;
    public IClipboard? Clipboard { get; set; }
    public List<ExportStudy> ExportStudies { get; } = [];
    public List<ExportStudy> SelectedExportStudies { get; } = [];
    public event Action<StudyPhrase>? OnSelectionChanged;
    public ExportStudiesViewModel(List<StudyPhrase> phrases)
    {
        ExportStudies.AddRange(phrases.Select(p => new ExportStudy(p)));
        Recal();
        this.PropertyChanged += (s, e) =>
        {
            if(e.PropertyName == nameof(SelectedStudy))
            {
                OnSelectionChanged?.Invoke(SelectedStudy?.Phrase);
            }
            if (e.PropertyName == nameof(IsSelectAll))
            {
                foreach (var es in ExportStudies)
                {
                    es.IsSelected = IsSelectAll;
                }
            }
            else if (e.PropertyName == nameof(IsInverseSelection))
            {
                foreach (var es in ExportStudies)
                {
                    es.IsSelected = !es.IsSelected;
                }
                IsInverseSelection = false;
            }
            else if (e.PropertyName == nameof(IncludeEnDefention) ||
                     e.PropertyName == nameof(IncludePhras) ||
                     e.PropertyName == nameof(IncludePhrasTranslation))
            {
                Recal();
            }
        };
         
    }



    [RelayCommand]
    void Export()
    {
        var text = string.Join(Environment.NewLine,
                               ExportStudies
                               .Where(es => es.IsSelected)
                               .Select((es,i) => $"{i+1,2}. {es.StudyPhrase}"));

        Clipboard?.SetTextAsync(text);
        this.CancelCommand.Execute(true);
    }

    void Recal()
    {
        foreach (var es in ExportStudies)
        {
            es.StudyPhrase = GetExportText(es.Phrase);
        }
    }
    string GetExportText(StudyPhrase phrase)
    {
        var orgin = !string.IsNullOrEmpty(phrase.Origin?.Trim()) ? $"{phrase.Origin} - " : "";
         

        var sb = new StringBuilder();

         sb.Append( $@"{orgin}{phrase.Content} ({phrase.StudyType}) {phrase.ArContentTranslation}");
        if(IncludeEnDefention && !string.IsNullOrEmpty(phrase.Translation?.Trim()))
        {
                sb.Append($"\n💡 {phrase.Translation}");
        }
        if (IncludePhras)
         {
                sb.Append($"\n🎬 {phrase.PhraseText}");
         }
        if (IncludePhrasTranslation && !string.IsNullOrEmpty(phrase.PhraseArTranslation?.Trim()))
        {
                    sb.Append($"\n👉 {phrase.PhraseArTranslation}");
        }

        return sb.ToString();
    }
}
