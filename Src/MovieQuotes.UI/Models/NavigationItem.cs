namespace MovieQuotes.UI.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using MovieQuotes.UI.ViewModels;
using System;

public partial class NavigationItem : ObservableObject
{
    public NavigationItem(string iconName, string displayName, Type pageType)
    {
        IconName = iconName;
        DisplayName = displayName;
        PageType = pageType;
    }

    public string IconName { get; }
    public string DisplayName { get; }
    public Type PageType { get; }

    [ObservableProperty]
    private bool _isActive;

    public bool Matches(PageViewModelBase page) => PageType.IsInstanceOfType(page);
}
