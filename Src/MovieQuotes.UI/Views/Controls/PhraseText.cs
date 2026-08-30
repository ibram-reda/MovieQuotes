namespace MovieQuotes.UI.Views.Controls;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Media;


public class PhraseText : TemplatedControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<PhraseText, string>(
            nameof(Text),
            defaultValue: string.Empty);

    public static readonly StyledProperty<string> HighlightProperty =
        AvaloniaProperty.Register<PhraseText, string>(
            nameof(Highlight),
            defaultValue: string.Empty);

    public static readonly StyledProperty<IBrush?> HighlightBackgroundProperty =
        AvaloniaProperty.Register<PhraseText, IBrush?>(
            nameof(HighlightBackground),
            Brushes.Yellow);

    public static readonly StyledProperty<IBrush?> HighlightForegroundProperty =
        AvaloniaProperty.Register<PhraseText, IBrush?>(
            nameof(HighlightForeground),
            Brushes.Black);

    public static readonly StyledProperty<FontWeight> HighlightFontWeightProperty =
        AvaloniaProperty.Register<PhraseText, FontWeight>(
            nameof(HighlightFontWeight),
            FontWeight.Bold);

    private TextBlock? _textBlock;

    static PhraseText()
    { 
        TextProperty.Changed.AddClassHandler<PhraseText>(
            static (control, _) => control.UpdateText());

        HighlightProperty.Changed.AddClassHandler<PhraseText>(
            static (control, _) => control.UpdateText());

        HighlightBackgroundProperty.Changed.AddClassHandler<PhraseText>(
            static (control, _) => control.UpdateText());

        HighlightForegroundProperty.Changed.AddClassHandler<PhraseText>(
            static (control, _) => control.UpdateText());

        HighlightFontWeightProperty.Changed.AddClassHandler<PhraseText>(
            static (control, _) => control.UpdateText());
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Highlight
    {
        get => GetValue(HighlightProperty);
        set => SetValue(HighlightProperty, value);
    }

    public IBrush? HighlightBackground
    {
        get => GetValue(HighlightBackgroundProperty);
        set => SetValue(HighlightBackgroundProperty, value);
    }

    public IBrush? HighlightForeground
    {
        get => GetValue(HighlightForegroundProperty);
        set => SetValue(HighlightForegroundProperty, value);
    }

    public FontWeight HighlightFontWeight
    {
        get => GetValue(HighlightFontWeightProperty);
        set => SetValue(HighlightFontWeightProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _textBlock = e.NameScope.Find<TextBlock>("PART_TextBlock");

        UpdateText();
    }

    private void UpdateText()
    {
        if (_textBlock is null)
            return;

        _textBlock.Inlines?.Clear();

        if (string.IsNullOrEmpty(Text))
            return;

        if (string.IsNullOrEmpty(Highlight))
        {
            _textBlock.Inlines?.Add(new Run(Text));
            return;
        }

        var currentIndex = 0;

        while (currentIndex < Text.Length)
        {
            var index = Text.IndexOf(
                Highlight,
                currentIndex,
                StringComparison.OrdinalIgnoreCase);

            // No more matches.
            if (index < 0)
            {
                AddNormalText(Text[currentIndex..]);
                break;
            }

            // Text before the highlighted word.
            if (index > currentIndex)
            {
                AddNormalText(Text[currentIndex..index]);
            }

            // Highlighted word.
            AddHighlightedText(Highlight);

            currentIndex = index + Highlight.Length;
        }
    }

    private void AddNormalText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        _textBlock?.Inlines?.Add(new Run(text));
    }

    private void AddHighlightedText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var run = new Run(text)
        {
            Background = HighlightBackground,
            Foreground = HighlightForeground,
            FontWeight = HighlightFontWeight
        };
        run.Classes.Add("highlighted");

        _textBlock?.Inlines?.Add(run);
    }
}