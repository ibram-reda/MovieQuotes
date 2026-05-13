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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MovieQuotes.UI.Extensions.StringExtensions;

public partial class ExportStudy : ObservableObject
{
    public ExportStudy(StudyPhrase phrase)
    {
        this.Phrase = phrase;
    }
    [ObservableProperty] bool isSelected = true;
    public StudyPhrase Phrase { get; }
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
    [ObservableProperty] bool includePronunciation;
    public IClipboard? Clipboard { get; set; }
    public List<ExportStudy> ExportStudies { get; } = [];
    public List<ExportStudy> SelectedExportStudies { get; } = [];

    public override string Title => "Export Studies";

    public event Action<StudyPhrase>? OnSelectionChanged;
    public ExportStudiesViewModel(List<StudyPhrase> phrases)
    {
        ExportStudies.AddRange(phrases.Select(p => new ExportStudy(p)));
        Recal();
        this.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedStudy))
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
                     e.PropertyName == nameof(IncludePhrasTranslation) ||
                     e.PropertyName == nameof(IncludePronunciation))
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
                               .Select((es, i) => $"{i + 1,2}. {es.StudyPhrase}"));

        Clipboard?.SetTextAsync(text);
        this.CancelCommand.Execute(true);
    }

    [RelayCommand]
    async Task ExportAnki()
    {
        var Header = """
            #separator:tab
            #html:true
            #notetype column:1
            #deck column:2
            
            """;
        var fileName = $"MoviePhrasesAnki.txt";
        var folderName = "MovieQuotes";
        var folderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), folderName);
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        var filePath = System.IO.Path.Combine(folderPath, fileName);
        using var writer = new System.IO.StreamWriter(filePath, false, Encoding.UTF8);

        writer.Write(Header);

        foreach (var study in ExportStudies.Where(es => !es.Phrase.IsDraft))
        {

            var line = GetExportTextAnki(study.Phrase);
            if (line.Contains("\""))
            {
                line = line.Replace("\"", "&quot;");
            }
            await writer.WriteAsync(line);
            await writer.WriteAsync(Environment.NewLine);
        }

        this.CancelCommand.Execute(true);
    }



    void Recal()
    {
        foreach (var es in ExportStudies)
        {
            es.StudyPhrase = GetExportText(es.Phrase);
        }
    }


    string GetExportTextAnki(StudyPhrase phrase)
    {
        var studyType = phrase.StudyType.RemoveSpace();
        var origin = phrase.Origin.RemoveSpace();
        var movieName = phrase.MovieName.RemoveSpace();
        var content = phrase.Content.RemoveSpace();
        var translation = phrase.Translation.GetOptionalMurkupList().ReplaceNewLine().RemoveSpace();
        var arContentTranslation = phrase.ArContentTranslation.ReplaceNewLine().RemoveSpace();
        var phraseText = phrase.PhraseText.ReplaceNewLine().RemoveSpace();
        var phraseArTranslation = phrase.PhraseArTranslation?.RemoveSpace();
        var examples = phrase.Examples?.GetMurkupList().RemoveSpace();
        var notes = phrase.Notes?.ReplaceNewLine()?.RemoveSpace();
        var pronunciation = phrase.Pronunciation?.RemoveSpace();
        var synonyms = phrase.Synonyms?.GetMurkupList()?.RemoveSpace();
        var level = phrase.Level;

        var v = $"moviwords\tMovie Words\t{phrase.StudyId}\t{studyType}\t{origin}\t{content}\t{pronunciation}\t{translation}\t{arContentTranslation}\t{phraseText}\t{phraseArTranslation}\t{examples}\t{notes}\t{movieName}\t{level}\t{synonyms}"
       .Replace("\n", "<br>");

        return v;
    }

    string GetExportText(StudyPhrase phrase)
    {
        var orgin = !string.IsNullOrEmpty(phrase.Origin?.Trim()) ? $"{phrase.Origin} - " : "";
        var pronounciation = IncludePronunciation && !string.IsNullOrEmpty(phrase.Pronunciation?.Trim()) ? $" {phrase.Pronunciation?.Trim()}" : "";

        var sb = new StringBuilder();

        sb.Append($@"{orgin}{phrase.Content}{pronounciation} ({phrase.StudyType}) {phrase.ArContentTranslation}");
        if (IncludeEnDefention && !string.IsNullOrEmpty(phrase.Translation?.Trim()))
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
