namespace MovieQuotes.UI.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MovieQuotes.UI.Models;
using MovieQuotes.UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;


public partial class MainWindowViewModel : ViewModelBase
{
    
    public MainWindowViewModel( )
    {
        NavigationService.NavigateTo<WelcomeScreenViewModel>();

        NavigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }

    private void OnCurrentViewModelChanged(ViewModelBase obj)
    {
        this.OnPropertyChanged(nameof(CurrentViewModel));
    }
     

    public ViewModelBase CurrentViewModel => NavigationService.CurrentViewModel;
}