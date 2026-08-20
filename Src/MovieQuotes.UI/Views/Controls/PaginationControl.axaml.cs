namespace MovieQuotes.UI.Views.Controls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

public class PaginationControl : TemplatedControl
{
    // ============================================================
    // Styled Properties
    // ============================================================

    public static readonly StyledProperty<uint> CurrentPageProperty =
        AvaloniaProperty.Register<PaginationControl, uint>(
            nameof(CurrentPage),
            defaultValue: 0);

    public static readonly StyledProperty<int> TotalCountProperty =
        AvaloniaProperty.Register<PaginationControl, int>(
            nameof(TotalCount),
            defaultValue: 0);

    public static readonly StyledProperty<uint> PageSizeProperty =
        AvaloniaProperty.Register<PaginationControl, uint>(
            nameof(PageSize),
            defaultValue: 20);

    public static readonly StyledProperty<ICommand?> PageChangedCommandProperty =
        AvaloniaProperty.Register<PaginationControl, ICommand?>(
            nameof(PageChangedCommand));


    // ============================================================
    // Direct Properties
    // ============================================================

    public static readonly DirectProperty<
        PaginationControl,
        IReadOnlyList<PaginationPageItem>> PageItemsProperty =
        AvaloniaProperty.RegisterDirect<
            PaginationControl,
            IReadOnlyList<PaginationPageItem>>(
                nameof(PageItems),
                control => control.PageItems);


    public static readonly DirectProperty<PaginationControl, uint>
        PageCountProperty =
        AvaloniaProperty.RegisterDirect<
            PaginationControl,
            uint>(
                nameof(PageCount),
                control => control.PageCount);


    public static readonly DirectProperty<PaginationControl, bool>
        HasPreviousPageProperty =
        AvaloniaProperty.RegisterDirect<
            PaginationControl,
            bool>(
                nameof(HasPreviousPage),
                control => control.HasPreviousPage);


    public static readonly DirectProperty<PaginationControl, bool>
        HasNextPageProperty =
        AvaloniaProperty.RegisterDirect<
            PaginationControl,
            bool>(
                nameof(HasNextPage),
                control => control.HasNextPage);


    public static readonly DirectProperty<
        PaginationControl,
        IRelayCommand> PreviousPageCommandProperty =
        AvaloniaProperty.RegisterDirect<
            PaginationControl,
            IRelayCommand>(
                nameof(PreviousPageCommand),
                control => control.PreviousPageCommand);


    public static readonly DirectProperty<
        PaginationControl,
        IRelayCommand> NextPageCommandProperty =
        AvaloniaProperty.RegisterDirect<
            PaginationControl,
            IRelayCommand>(
                nameof(NextPageCommand),
                control => control.NextPageCommand);


    // ============================================================
    // Backing Fields
    // ============================================================

    private IReadOnlyList<PaginationPageItem> _pageItems =
        Array.Empty<PaginationPageItem>();

    private uint _pageCount;

    private bool _hasPreviousPage;

    private bool _hasNextPage;


    // ============================================================
    // Properties
    // ============================================================

    public uint CurrentPage
    {
        get => GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }


    public int TotalCount
    {
        get => GetValue(TotalCountProperty);
        set => SetValue(TotalCountProperty, value);
    }


    public uint PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }


    public ICommand? PageChangedCommand
    {
        get => GetValue(PageChangedCommandProperty);
        set => SetValue(PageChangedCommandProperty, value);
    }


    public IReadOnlyList<PaginationPageItem> PageItems
    {
        get => _pageItems;

        private set =>
            SetAndRaise(
                PageItemsProperty,
                ref _pageItems,
                value);
    }


    public uint PageCount
    {
        get => _pageCount;

        private set =>
            SetAndRaise(
                PageCountProperty,
                ref _pageCount,
                value);
    }


    public bool HasPreviousPage
    {
        get => _hasPreviousPage;

        private set =>
            SetAndRaise(
                HasPreviousPageProperty,
                ref _hasPreviousPage,
                value);
    }


    public bool HasNextPage
    {
        get => _hasNextPage;

        private set =>
            SetAndRaise(
                HasNextPageProperty,
                ref _hasNextPage,
                value);
    }


    public IRelayCommand PreviousPageCommand { get; }

    public IRelayCommand NextPageCommand { get; }

    public IRelayCommand<uint> SelectPageCommand { get; }


    // ============================================================
    // Constructor
    // ============================================================

    public PaginationControl()
    {
        PreviousPageCommand =
            new RelayCommand(
                PreviousPage,
                () => HasPreviousPage);


        NextPageCommand =
            new RelayCommand(
                NextPage,
                () => HasNextPage);


        SelectPageCommand =
            new RelayCommand<uint>(
                SelectPage);


        CurrentPageProperty.Changed.AddClassHandler<PaginationControl>(
            static (control, _) =>
                control.Refresh());


        TotalCountProperty.Changed.AddClassHandler<PaginationControl>(
            static (control, _) =>
                control.Refresh());


        PageSizeProperty.Changed.AddClassHandler<PaginationControl>(
            static (control, _) =>
                control.Refresh());


        Refresh();
    }


    // ============================================================
    // Navigation
    // ============================================================

    private void PreviousPage()
    {
        if (!HasPreviousPage)
            return;

        PageChangedCommand?.Execute(
            CurrentPage - 1);
    }


    private void NextPage()
    {
        if (!HasNextPage)
            return;

        PageChangedCommand?.Execute(
            CurrentPage + 1);
    }


    private void SelectPage(uint page)
    {
        if (page >= PageCount)
            return;

        if (page == CurrentPage)
            return;

        PageChangedCommand?.Execute(page);
    }


    // ============================================================
    // Refresh
    // ============================================================

    private void Refresh()
    {
        var pageCount = CalculatePageCount();

        PageCount = pageCount;


        if (pageCount == 0)
        {
            HasPreviousPage = false;
            HasNextPage = false;

            PageItems = Array.Empty<PaginationPageItem>();

            NotifyCommands();

            return;
        }


        // Prevent CurrentPage from going outside the valid range.
        if (CurrentPage >= pageCount)
        {
            CurrentPage = pageCount - 1;

            return;
        }


        HasPreviousPage =
            CurrentPage > 0;


        HasNextPage =
            CurrentPage + 1 < pageCount;


        PageItems =
            BuildPageItems(pageCount);


        NotifyCommands();
    }


    private uint CalculatePageCount()
    {
        if (PageSize == 0)
            return 0;

        if (TotalCount <= 0)
            return 0;

        return (uint)Math.Ceiling(
            TotalCount / (double)PageSize);
    }


    // ============================================================
    // Page Items
    // ============================================================

    private IReadOnlyList<PaginationPageItem>
        BuildPageItems(uint pageCount)
    {
        // Show every page if there aren't many.
        if (pageCount <= 7)
        {
            var result =
                new List<PaginationPageItem>(
                    (int)pageCount);

            for (uint page = 0;
                 page < pageCount;
                 page++)
            {
                result.Add(
                    CreatePageItem(page));
            }

            return result;
        }


        var pageNumbers =
            new SortedSet<uint>
            {
                0,
                pageCount - 1
            };


        // Current page and neighbors.
        var firstVisiblePage =
            CurrentPage > 1
                ? CurrentPage - 1
                : 1;


        var lastVisiblePage =
            Math.Min(
                pageCount - 2,
                CurrentPage + 1);


        for (var page = firstVisiblePage;
             page <= lastVisiblePage;
             page++)
        {
            pageNumbers.Add(page);
        }


        var resultItems =
            new List<PaginationPageItem>();


        uint? previousPage = null;


        foreach (var page in pageNumbers)
        {
            // There is a gap.
            if (previousPage.HasValue &&
                page > previousPage.Value + 1)
            {
                resultItems.Add(
                    PaginationPageItem.Ellipsis());
            }


            resultItems.Add(
                CreatePageItem(page));


            previousPage = page;
        }


        return resultItems;
    }


    private PaginationPageItem CreatePageItem(
        uint page)
    {
        return new PaginationPageItem(
            pageNumber: page,
            display: (page + 1).ToString(),
            isCurrent: page == CurrentPage,
            command: SelectPageCommand,
            commandParameter: page);
    }


    // ============================================================
    // Commands
    // ============================================================

    private void NotifyCommands()
    {
        PreviousPageCommand
            .NotifyCanExecuteChanged();

        NextPageCommand
            .NotifyCanExecuteChanged();
    }
}