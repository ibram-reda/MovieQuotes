namespace MovieQuotes.UI.Views.Controls;

using System.Windows.Input;

public sealed class PaginationPageItem
{
    private PaginationPageItem(
        uint pageNumber,
        string display,
        bool isCurrent,
        ICommand? command,
        object? commandParameter,
        bool isEllipsis)
    {
        PageNumber = pageNumber;
        Display = display;
        IsCurrent = isCurrent;
        Command = command;
        CommandParameter = commandParameter;
        IsEllipsis = isEllipsis;
    }

    public uint PageNumber { get; }

    public string Display { get; }

    public bool IsCurrent { get; }

    public bool IsEllipsis { get; }

    public bool IsEnabled =>
        !IsCurrent &&
        !IsEllipsis;

    public ICommand? Command { get; }

    public object? CommandParameter { get; }

    public PaginationPageItem(
        uint pageNumber,
        string display,
        bool isCurrent,
        ICommand command,
        object commandParameter)
        : this(
            pageNumber,
            display,
            isCurrent,
            command,
            commandParameter,
            false)
    {
    }

    public static PaginationPageItem Ellipsis()
    {
        return new PaginationPageItem(
            pageNumber: 0,
            display: "..",
            isCurrent: false,
            command: null,
            commandParameter: null,
            isEllipsis: true);
    }
}