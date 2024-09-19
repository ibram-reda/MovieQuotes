namespace MovieQuotes.UI.ViewModels;

using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieQuotes.Application.Models;
using MovieQuotes.Application.Operations.Commands; 
using MovieQuotes.UI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


internal partial class NewMovieViewModel : ViewModelBase
{
    private readonly IFilesService filesService;
    private bool returnToPreviousPageAfterSave = false; 
    [ObservableProperty] private string _movieVideoPath = string.Empty;
    [ObservableProperty] private string _movieName = string.Empty;
    [ObservableProperty] private string _movieSubtitlePath = string.Empty;
    [ObservableProperty] private string? _IMDBId = string.Empty;
    [ObservableProperty] private string? _coverURl = string.Empty;
    [ObservableProperty] private string? _description = string.Empty;

    public override string Title => "➕ insert new movie";

    public NewMovieViewModel()
    {
        this.filesService = this.GetService<IFilesService>();

    }

    [RelayCommand]
    private async Task LoadSubtitle(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var file = await filesService.OpenFileAsync("select subtitleFolder");
            if (file is null) return;
            MovieSubtitlePath = file.Path.LocalPath;  
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task LoadCover(CancellationToken token)
    {
        ErrorMessages?.Clear();
        try
        {
            var file = await filesService.OpenFileAsync("select Cover Photo", [FilePickerFileTypes.ImageAll]);
            if (file is null) return;
            CoverURl = file.Path.LocalPath; 
        }
        catch (Exception e)
        {
            ErrorMessages?.Add(e.Message);
        }
    }

    [RelayCommand]
    private async Task LoadVideo(CancellationToken token)
    {
        var file = await filesService.OpenFileAsync("Select movie file");
        if (file is null) return;
        MovieVideoPath = file.Path.LocalPath;
        MovieName = file.Name.Remove(file.Name.LastIndexOf('.'));
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task SaveIntoDb(CancellationToken token = default)
    {
        ErrorMessages?.Clear();
        var command = new CreateMovieCommand(MovieName, MovieVideoPath, MovieSubtitlePath, Description, IMDBId, CoverURl);

        IsBusy = true;
        var result = await mediator.Send(command);
        IsBusy = false;

        if (result.IsError)
        {
            foreach (var error in result.Errors)
                ErrorMessages?.Add(error.Message);
            return;
        }

        if(returnToPreviousPageAfterSave) 
            this.NavigationService.GoBack(MovieName);

        ResetProperties();
    }

    public override void Init(object? initValue)
    {
        if (initValue is MovieInfo info) 
        {
            this.returnToPreviousPageAfterSave = true;
            MovieVideoPath = info.LocalPath;
            MovieSubtitlePath = info.SubtitlePath??"";
            IMDBId = info.IMDBId;
            MovieName = info.Title;
            CoverURl = info.CoverUrl;
            Description = info.Description;
        }
    }
    private void ResetProperties()
    { 
        MovieVideoPath = string.Empty;
        MovieSubtitlePath = string.Empty;
        IMDBId = string.Empty;
        MovieName = string.Empty;
        CoverURl = string.Empty;
        Description = string.Empty;
    }
}
